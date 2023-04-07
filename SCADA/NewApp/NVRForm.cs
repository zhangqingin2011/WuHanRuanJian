using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SCADA.NewApp
{
    public partial class NVRForm : Form
    {
        public NVRForm()
        {
            InitializeComponent();
        }

        public delegate void JpegHandle(object sender, int index, string picpath);
        public static JpegHandle jpegpic;

        void OnJpegAction(int index, string picpath)
        {
            if (jpegpic != null)
            {
                jpegpic(this, index, picpath);
            }
        }

        private enum IPCamera
        {
            车床,
            铣床
        }

        public struct CHAN_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 256, ArraySubType = UnmanagedType.U4)]
            public Int32[] lChannelNo;
            public void Init()
            {
                lChannelNo = new Int32[256];
                for (int i = 0; i < 256; i++)
                    lChannelNo[i] = -1;
            }
        }

        private struct NVR
        {
            public string IP;
            public Int32 UserIDForUnit;
            public Int32 RealHandleLathe;
            public Int32 RealHandleCenter;
            public CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
            public CHCNetSDK.NET_DVR_IPPARACFG_V40 m_struIpParaCfgV40;
            public CHAN_INFO m_struChanNoInfo;

            public void Init()
            {
                UserIDForUnit = -1;
                RealHandleLathe = -1;
                RealHandleCenter = -1;
                m_struDeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                m_struChanNoInfo = new CHAN_INFO();
                m_struChanNoInfo.Init();
            }
        }

        private NVR nvr1 = new NVR();
        private NVR nvr2 = new NVR();
        private NVR nvr3 = new NVR();
        private NVR nvr4 = new NVR();
        private string username;
        Int16 Port;
        ConfigXmlReader xmlReader = new ConfigXmlReader();

        private void InitConfig()
        {
            comboBox1.Items.Add("加工单元1车床");
            comboBox2.Items.Add("加工单元1车床");
            comboBox1.Items.Add("加工单元1铣床");
            comboBox2.Items.Add("加工单元1铣床");
            comboBox1.Items.Add("加工单元2车床");
            comboBox2.Items.Add("加工单元2车床");
            comboBox1.Items.Add("加工单元2铣床");
            comboBox2.Items.Add("加工单元2铣床");
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.Items.Add("拓展单元1车床");
            comboBox3.Items.Add("拓展单元1铣床");
            comboBox3.Items.Add("拓展单元2车床");
            comboBox3.Items.Add("拓展单元2铣床");
            comboBox4.Items.Add("拓展单元1车床");
            comboBox4.Items.Add("拓展单元1铣床");
            comboBox4.Items.Add("拓展单元1车床");
            comboBox4.Items.Add("拓展单元1铣床");
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            nvr1.Init();
            nvr2.Init();
            nvr3.Init();
            nvr4.Init();
            xmlReader.ReadXml(@"..\data\video\Config.xml");
            username = xmlReader.configDictionary["User"];
            nvr1.IP = xmlReader.configDictionary["NVR1"];
            nvr2.IP = xmlReader.configDictionary["NVR2"];
            nvr3.IP = xmlReader.configDictionary["NVR3"];
            nvr4.IP = xmlReader.configDictionary["NVR4"];
            Port = Int16.Parse(xmlReader.configDictionary["PORT"]);
            Console.WriteLine("pictureBox1:{0},pictureBox2:{1},pictureBox3:{2},pictureBox4:{3}", pictureBox1.Handle, pictureBox2.Handle, pictureBox3.Handle, pictureBox4.Handle);
            bool res = CHCNetSDK.NET_DVR_Init();
            if (res)
                PushMessage(richTextBox1, "录像设备初始化成功!", Color.Black);
            else
                PushMessage(richTextBox1, "录像设备初始化失败!", Color.Red);
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private void PushMessage(RichTextBox richTextBox, string Msg, Color color)
        {
            richTextBox.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                c.SelectionColor = color;
                c.AppendText(Message);
            });
        }

        private void Login(RichTextBox richTextBox, string passwd, ref NVR nvr, int no)
        {
            if (nvr.UserIDForUnit >= 0)
            {
                PushMessage(richTextBox, "录像机" + no.ToString() + "已登录！", Color.Black);
                return;
            }

            nvr.UserIDForUnit = CHCNetSDK.NET_DVR_Login_V30(nvr.IP, Port, username, passwd, ref nvr.m_struDeviceInfo);
            if (nvr.UserIDForUnit < 0)
            {
                uint error = CHCNetSDK.NET_DVR_GetLastError();
                PushMessage(richTextBox, string.Format("录像机{0}登录失败！错误号:{1}.", no, error), Color.Red);
            }
            else
            {
                Console.WriteLine(nvr.UserIDForUnit);
                PushMessage(richTextBox, string.Format("录像机{0}登录成功！", no), Color.Black);
                /***获取通道***/
                uint dwDChanTotalNum = (uint)nvr.m_struDeviceInfo.byIPChanNum + 256 * (uint)nvr.m_struDeviceInfo.byHighDChanNum;
                if (dwDChanTotalNum > 0)
                {
                    uint dwSize = (uint)Marshal.SizeOf(nvr.m_struIpParaCfgV40);

                    IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
                    Marshal.StructureToPtr(nvr.m_struIpParaCfgV40, ptrIpParaCfgV40, false);

                    uint dwReturn = 0;
                    int iGroupNo = 0;  //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取

                    if (!CHCNetSDK.NET_DVR_GetDVRConfig(nvr.UserIDForUnit, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox, string.Format("录像机{0}获取IP通道信息失败, 错误号{1}。", no, error), Color.Red);
                    }
                    else
                    {
                        int i = 0, j = 0;
                        nvr.m_struIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));

                        //获取可用的模拟通道
                        for (i = 0; i < nvr.m_struIpParaCfgV40.dwAChanNum; i++)
                        {
                            if (nvr.m_struIpParaCfgV40.byAnalogChanEnable[i] == 1)
                            {
                                nvr.m_struChanNoInfo.lChannelNo[j] = i + nvr.m_struDeviceInfo.byStartChan;
                                j++;
                            }
                        }

                        //获取前64个IP通道中的在线通道
                        uint iDChanNum = 64;

                        if (dwDChanTotalNum < 64)
                        {
                            iDChanNum = dwDChanTotalNum; //如果设备IP通道小于64路，按实际路数获取
                        }

                        byte byStreamType;
                        for (i = 0; i < iDChanNum; i++)
                        {
                            byStreamType = nvr.m_struIpParaCfgV40.struStreamMode[i].byGetStreamType;
                            CHCNetSDK.NET_DVR_STREAM_MODE m_struStreamMode = new CHCNetSDK.NET_DVR_STREAM_MODE();
                            dwSize = (uint)Marshal.SizeOf(m_struStreamMode);
                            switch (byStreamType)
                            {
                                //0- 直接从设备取流 0- get stream from device directly
                                case 0:
                                    IntPtr ptrChanInfo = Marshal.AllocHGlobal((Int32)dwSize);
                                    Marshal.StructureToPtr(nvr.m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfo, false);
                                    CHCNetSDK.NET_DVR_IPCHANINFO m_struChanInfo = new CHCNetSDK.NET_DVR_IPCHANINFO();
                                    m_struChanInfo = (CHCNetSDK.NET_DVR_IPCHANINFO)Marshal.PtrToStructure(ptrChanInfo, typeof(CHCNetSDK.NET_DVR_IPCHANINFO));

                                    //列出IP通道 List the IP channel
                                    if (m_struChanInfo.byEnable == 1)
                                    {
                                        nvr.m_struChanNoInfo.lChannelNo[j] = i + (int)nvr.m_struIpParaCfgV40.dwStartDChan;
                                        Console.WriteLine("nvr.m_struChanNoInfo{0},{1}", j, nvr.m_struChanNoInfo.lChannelNo[j]);
                                        j++;
                                    }
                                    Marshal.FreeHGlobal(ptrChanInfo);
                                    break;
                                //6- 直接从设备取流扩展 6- get stream from device directly(extended)
                                case 6:
                                    IntPtr ptrChanInfoV40 = Marshal.AllocHGlobal((Int32)dwSize);
                                    Marshal.StructureToPtr(nvr.m_struIpParaCfgV40.struStreamMode[i].uGetStream, ptrChanInfoV40, false);
                                    CHCNetSDK.NET_DVR_IPCHANINFO_V40 m_struChanInfoV40 = new CHCNetSDK.NET_DVR_IPCHANINFO_V40();
                                    m_struChanInfoV40 = (CHCNetSDK.NET_DVR_IPCHANINFO_V40)Marshal.PtrToStructure(ptrChanInfoV40, typeof(CHCNetSDK.NET_DVR_IPCHANINFO_V40));

                                    //列出IP通道 List the IP channel
                                    if (m_struChanInfoV40.byEnable == 1)
                                    {
                                        nvr.m_struChanNoInfo.lChannelNo[j] = i + (int)nvr.m_struIpParaCfgV40.dwStartDChan;
                                        j++;
                                    }
                                    Marshal.FreeHGlobal(ptrChanInfoV40);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    Marshal.FreeHGlobal(ptrIpParaCfgV40);
                }
                else
                {
                    int i = 0, j = 0;
                    for (i = 0; i < nvr.m_struDeviceInfo.byChanNum; i++)
                    {
                        nvr.m_struChanNoInfo.lChannelNo[j] = i + nvr.m_struDeviceInfo.byStartChan;
                        j++;
                    }
                }
            }
        }

        private void Btnlogin_Click(object sender, EventArgs e)
        {
            Login(richTextBox1, textBox1.Text, ref nvr1, 1);
            Login(richTextBox1, textBox1.Text, ref nvr2, 2);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int curchannel = 0;
            if (comboBox1.SelectedIndex == 0)
            {
                curchannel = nvr1.m_struChanNoInfo.lChannelNo[0];
                Console.WriteLine("pictureBox1:{0}", pictureBox1.Handle);
                if (nvr1.RealHandleLathe < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox1.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr1.RealHandleLathe = CHCNetSDK.NET_DVR_RealPlay_V40(nvr1.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr1.RealHandleLathe < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox1, string.Format("录像机1的车床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox1, "录像机1的车床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox1, "录像机1的车床正在预览!", Color.Black);
                }
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                curchannel = nvr1.m_struChanNoInfo.lChannelNo[1];
                Console.WriteLine("pictureBox2:{0}", pictureBox2.Handle);
                if (nvr1.RealHandleCenter < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox2.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr1.RealHandleCenter = CHCNetSDK.NET_DVR_RealPlay_V40(nvr1.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr1.RealHandleCenter < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox1, string.Format("录像机1的铣床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox1, "录像机1的铣床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox1, "录像机1的铣床正在预览!", Color.Black);
                }
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                curchannel = nvr2.m_struChanNoInfo.lChannelNo[0];
                Console.WriteLine(curchannel);
                Console.WriteLine("pictureBox3:{0}", pictureBox3.Handle);
                if (nvr2.RealHandleLathe < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox3.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = (uint)1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr2.RealHandleLathe = CHCNetSDK.NET_DVR_RealPlay_V40(nvr2.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    Console.WriteLine("{0}, {1}", nvr2.RealHandleLathe, nvr2.UserIDForUnit);
                    if (nvr2.RealHandleLathe < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox1, string.Format("录像机2的车床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox1, "录像机2的车床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox1, "录像机2的车床正在预览!", Color.Black);
                }
            }
            else if (comboBox1.SelectedIndex == 3)
            {
                curchannel = nvr2.m_struChanNoInfo.lChannelNo[1];
                Console.WriteLine("pictureBox4:{0}", pictureBox4.Handle);
                if (nvr2.RealHandleCenter < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox4.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr2.RealHandleCenter = CHCNetSDK.NET_DVR_RealPlay_V40(nvr2.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr2.RealHandleCenter < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox1, string.Format("录像机2的铣床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox1, "录像机2的铣床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox1, "录像机2的铣床正在预览!", Color.Black);
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            int curchannel = 0;
            string sJpegPicFileName;
            string timeday = DateTime.Now.ToString();
            timeday = timeday.Replace("/", "");
            timeday = timeday.Replace(":", "");
            timeday = timeday.Replace(" ", "");
            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档
            if (comboBox2.SelectedIndex == 0)
            {
                curchannel = nvr1.m_struChanNoInfo.lChannelNo[0];
                sJpegPicFileName = "../data/jpeg/" + "Unit1Lathe" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr1.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox1, string.Format("录像机1的车床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox2.SelectedIndex, sJpegPicFileName);
                    PushMessage(richTextBox1, string.Format("录像机1的车床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox2.SelectedIndex == 1)
            {
                curchannel = nvr1.m_struChanNoInfo.lChannelNo[1];
                sJpegPicFileName = "../data/jpeg/" + "Unit1Center" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr1.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox1, string.Format("录像机1的铣床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox2.SelectedIndex, sJpegPicFileName);
                    PushMessage(richTextBox1, string.Format("录像机1的铣床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox2.SelectedIndex == 2)
            {
                curchannel = nvr2.m_struChanNoInfo.lChannelNo[0];
                sJpegPicFileName = "../data/jpeg/" + "Unit2Lathe" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr2.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox1, string.Format("录像机2的车床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox2.SelectedIndex, sJpegPicFileName);
                    PushMessage(richTextBox1, string.Format("录像机2的车床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox2.SelectedIndex == 3)
            {
                curchannel = nvr2.m_struChanNoInfo.lChannelNo[1];
                sJpegPicFileName = "../data/jpeg/" + "Unit2Center" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr2.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox1, string.Format("录像机2的铣床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox2.SelectedIndex, sJpegPicFileName);
                    PushMessage(richTextBox1, string.Format("录像机2的铣床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
        }

        private void NVRForm_Load(object sender, EventArgs e)
        {
            InitConfig();
            //tabControl1.TabPages.Remove(tabPage2);
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Login(richTextBox2, textBox2.Text, ref nvr3, 3);
            Login(richTextBox2, textBox2.Text, ref nvr4, 4);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int curchannel = 0;
            if (comboBox3.SelectedIndex == 0)
            {
                curchannel = nvr3.m_struChanNoInfo.lChannelNo[0];
                Console.WriteLine("pictureBox5:{0}", pictureBox5.Handle);
                if (nvr3.RealHandleLathe < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox5.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr3.RealHandleLathe = CHCNetSDK.NET_DVR_RealPlay_V40(nvr3.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr3.RealHandleLathe < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox2, string.Format("录像机3的车床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox2, "录像机3的车床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox2, "录像机3的车床正在预览!", Color.Black);
                }
            }
            else if (comboBox3.SelectedIndex == 1)
            {
                curchannel = nvr3.m_struChanNoInfo.lChannelNo[1];
                Console.WriteLine("pictureBox6:{0}", pictureBox6.Handle);
                if (nvr3.RealHandleCenter < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox6.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr3.RealHandleCenter = CHCNetSDK.NET_DVR_RealPlay_V40(nvr3.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr3.RealHandleCenter < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox2, string.Format("录像机3的铣床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox2, "录像机3的铣床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox2, "录像机3的铣床正在预览!", Color.Black);
                }
            }
            else if (comboBox3.SelectedIndex == 2)
            {
                curchannel = nvr4.m_struChanNoInfo.lChannelNo[0];
                Console.WriteLine(curchannel);
                Console.WriteLine("pictureBox7:{0}", pictureBox7.Handle);
                if (nvr4.RealHandleLathe < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox7.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = (uint)1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr4.RealHandleLathe = CHCNetSDK.NET_DVR_RealPlay_V40(nvr4.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    //Console.WriteLine("{0}, {1}", nvr4.RealHandleLathe, nvr4.UserIDForUnit);
                    if (nvr4.RealHandleLathe < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox2, string.Format("录像机4的车床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox2, "录像机4的车床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox2, "录像机4的车床正在预览!", Color.Black);
                }
            }
            else if (comboBox3.SelectedIndex == 3)
            {
                curchannel = nvr4.m_struChanNoInfo.lChannelNo[1];
                Console.WriteLine("pictureBox4:{0}", pictureBox8.Handle);
                if (nvr4.RealHandleCenter < 0)
                {
                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo1 = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo1.hPlayWnd = pictureBox8.Handle;//预览窗口
                    lpPreviewInfo1.lChannel = curchannel;//预te览的设备通道
                    lpPreviewInfo1.dwStreamType = 1;//(uint)comboBoxcode.SelectedIndex;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo1.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo1.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    lpPreviewInfo1.dwDisplayBufNum = 5; //播放库播放缓冲区最大缓冲帧数  
                    /*if (RealData == null)
                    {
                        RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                    }*/
                    IntPtr pUser = new IntPtr();//用户数据
                    //打开预览 Start live view 
                    nvr4.RealHandleCenter = CHCNetSDK.NET_DVR_RealPlay_V40(nvr4.UserIDForUnit, ref lpPreviewInfo1, null/*RealData*/, pUser);
                    if (nvr4.RealHandleCenter < 0)
                    {
                        uint error = CHCNetSDK.NET_DVR_GetLastError();
                        PushMessage(richTextBox2, string.Format("录像机4的铣床预览失败，错误号{0}.", error), Color.Red);
                        return;
                    }
                    else
                    {
                        PushMessage(richTextBox2, "录像机4的铣床预览成功!", Color.Black);
                    }
                }
                else
                {
                    PushMessage(richTextBox2, "录像机4的铣床正在预览!", Color.Black);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int curchannel = 0;
            string sJpegPicFileName;
            string timeday = DateTime.Now.ToString();
            timeday = timeday.Replace("/", "");
            timeday = timeday.Replace(":", "");
            timeday = timeday.Replace(" ", "");
            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档
            if (comboBox4.SelectedIndex == 0)
            {
                curchannel = nvr3.m_struChanNoInfo.lChannelNo[0];
                sJpegPicFileName = "../data/jpeg/" + "Unit3Lathe" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr3.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox2, string.Format("录像机3的车床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox4.SelectedIndex + 4, sJpegPicFileName);
                    PushMessage(richTextBox2, string.Format("录像机3的车床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox4.SelectedIndex == 1)
            {
                curchannel = nvr3.m_struChanNoInfo.lChannelNo[1];
                sJpegPicFileName = "../data/jpeg/" + "Unit3Center" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr3.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox2, string.Format("录像机3的铣床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox4.SelectedIndex + 4, sJpegPicFileName);
                    PushMessage(richTextBox2, string.Format("录像机3的铣床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox4.SelectedIndex == 2)
            {
                curchannel = nvr4.m_struChanNoInfo.lChannelNo[0];
                sJpegPicFileName = "../data/jpeg/" + "Unit4Lathe" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr4.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox2, string.Format("录像机4的车床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox4.SelectedIndex + 4, sJpegPicFileName);
                    PushMessage(richTextBox2, string.Format("录像机4的车床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
            else if (comboBox4.SelectedIndex == 3)
            {
                curchannel = nvr4.m_struChanNoInfo.lChannelNo[1];
                sJpegPicFileName = "../data/jpeg/" + "Unit2Center" + timeday + ".jpg";
                //JPEG抓图 Capture a JPEG picture
                if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(nvr4.UserIDForUnit, curchannel, ref lpJpegPara, sJpegPicFileName))
                {
                    uint error = CHCNetSDK.NET_DVR_GetLastError();
                    PushMessage(richTextBox2, string.Format("录像机4的铣床抓图失败, 错误号:{0}", error), Color.Red);
                }
                else
                {
                    OnJpegAction(comboBox4.SelectedIndex + 4, sJpegPicFileName);
                    PushMessage(richTextBox2, string.Format("录像机4的铣床抓图成功, 图片路径:{0}", sJpegPicFileName), Color.Black);
                }
            }
        }
    }
}
