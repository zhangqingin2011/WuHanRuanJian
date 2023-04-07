using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public class ModbusTCP
    {
        protected TcpClient ModbusClient = null; //modbus TCP客户端
        public int[] MoubusPLCReg = null;//与西门子通讯寄存器存储位置
        public byte[][] ReceiveCache = new byte[1000][];  //定义内存的接收缓冲区的数组
        private int Port = 502;
        private string IP = string.Empty;

        private int TransferFlag = 1;  // 传输标志默认定义1，然后累加至1000后回到1循环，用于请求与回应对应
        //private static int DeviceNumber = 1;  // 设备编号默认定义1
        protected static int recievesize = 512;  //接收缓冲区大小
        private object locker = new object();
        private static int ComunicationRegNum = 300; //定义预留的寄存器个数

        public struct MBAPHead
        {
            public byte TransHigh; // 传输标志高字节（传输标志默认定义1）
            public byte TransLow;  // 传输标志低字节（传输标志默认定义1）
            public byte ArgreeHigh; // 协议标志高字节 （默认0，为modbus协议）
            public byte ArgreeLow;  // 协议标志低字节 （默认0，为modbus协议）
            public byte LengthHigh;  // 后续长度高字节
            public byte LengthLow;  // 后续长度低字节
            public byte DeviceNum;  // 设备编号 （默认定义1）
        }

        public enum FUNCODE : byte
        {
            读多个寄存器 = 0x03,
            写单个寄存器 = 0x06,
            写多个寄存器 = 0x10,
            读多个寄存器错误 = 0x03 + 0x80,
            写单个寄存器错误 = 0x06 + 0x80,
            写多个寄存器错误 = 0x10 + 0x80
        }

        public enum DATAOFFSET : int
        {
            传输标志起始 = 0,
            协议标志起始 = 2,
            长度起始 = 4,
            设备编号起始 = 6,
            功能码起始 = 7,
        }

        private enum DATATABLE
        {
            MES发给PLC的命令 = 0,
            MES发给PLC的机床下料仓位n,
            MES发给PLC设备号k,
            MES发给PLC的机床上料仓位m,
            MES发给PLC的命令2,
            MES发给PLC取成品底盘仓位a,
            MES发给PLC取成品圆盘仓位b,
            装配料盘出库去装配,         //125表示料盘需要出库去装配
            MES响应车床加工完 = 10,
            MES响应上料仓位m,
            MES响应下料仓位n,
            MES响应设备k,
            MES响应PLC发送的命令,
            MES响应加工中心加工完,
            MES响应上料仓位m2,
            MES响应下料仓位n2,
            MES响应设备k2,
            MES响应PLC发送CCD的拍照结果,
            PLC向MES发送命令车床加工完 = 20,
            PLC向MES发送的上料位值m,
            PLC向MES发送的下料位值n,
            PLC向MES发送的设备k,
            PLC向MES发送的命令,
            PLC向MES发送命令加工中心加工完,
            PLC向MES发送的上料位值m2,
            PLC向MES发送的下料位值n2,
            PLC向MES发送的设备k2,
            PLC向MES发送CCD的拍照结果,
            PLC响应MES命令 = 30,
            PLC响应MES料位机床下料仓位n,
            PLC响应MES设备号k,
            PLC响应MES料位机床上料仓位m,
            PLC响应MES的命令2,
            PLC响应MES取成品底盘仓位a,
            PLC响应MES取成品圆盘仓位b,
            机械手的状态 = 40,
            机械手是否在HOME位置确认,
            机械手运行模式,
            机器人繁忙,
            机械手关节1的坐标值,
            机械手关节2的坐标值,
            机械手关节3的坐标值,
            机械手关节4的坐标值,
            机械手关节5的坐标值,
            机械手关节6的坐标值,
            机械手关节7的坐标值 = 50,
            机械手当前使用的夹爪,   //1方料，2大圆，3小圆，4（新增）
            车床加工完成状态,       //1为完成
            加工中心加工完成状态,     //1为完成
            料仓仓位状态1 = 60,    //高字节对应1-8仓位，低字节对应9-16仓位
            料仓仓位状态2,     //高字节对应17-24仓位，低字节对应25-30仓位
            车床状态 = 65,      //0,1,2位对应车床的自动门关闭，打开，卡盘状态；
            加工中心状态,       //0,1,2对应加工中心的自动门关闭，打开，卡盘状态
            AGV与站台信号,    //第0位表示AGV到站台1，第1位表示AGV到站台2，第2位表示AGV上有料盘
            仓位1场次 = 70,
            仓位1编号,
            仓位1零件类型,
            仓位1零件状态,     //0空，1待加工，2正在加工，3合格品，4不合格品，5车床加工完成，6加工中心加工完成，7异常状态
            装配料仓仓位状态1 = 190,    //高字节对应1-8仓位，低字节对应9-16仓位
            装配料仓仓位状态2,    //高字节对应17-24仓位，低字节对应25-30仓位
            中转台工位光电         //中转台1的四个光电在0,1,2,3位，中转台2的四个光电在4,5,6,7位
        }

        public enum COMMAND1
        {
            启动系统 = 98,
            停止系统,
            启动设备,
            加工调度 = 102,
            写RFID信息,
            读RFID信息,
            返修
        }

        public enum COMMAND2
        {
            装配 = 120,
            RFID1读完,
            RFID1写完,
            RFID2读完,
            RFID2写完
        }

        public enum DEVICENO
        {
            车床 = 1,
            加工中心
        }

        public enum RFIDSIGNAL
        {
            请求读RFID1 = 121,
            请求写RFID1,
            请求读RFID2,
            请求写RFID2
        }

        /// <summary>
        /// 初始化缓冲区
        /// </summary>
        public void InitReceiveCache()
        {
            MoubusPLCReg = new int[ComunicationRegNum];
            for (int i = 0; i < 1000; i++)
            {
                ReceiveCache[i] = new byte[recievesize];
                for (int j = 0; j < recievesize; j++)
                {
                    ReceiveCache[i][j] = 0;
                }
            }
        }

        /// <summary>
        /// 设置端口
        /// </summary>
        /// <param name="port"></param>
        public void SetPort(int port)
        {
            Port = port;
        }
        /// <summary>
        /// 设置连接的Modbus的IP
        /// </summary>
        /// <param name="ip"></param>
        public void SetIP(string ip)
        {
            IP = ip;
        }

        /// <summary>
        /// 检测是否在线
        /// </summary>
        /// <returns></returns>
        public bool CheckOnline()
        {
            if (ModbusClient != null)
            {
                try
                {
                    if (ModbusClient.Connected)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// 测试IP是否在局域网内
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <returns></returns>
        private bool PingTest(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 300);
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

        /// <summary>
        /// 连接IP
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <returns></returns>
        private bool ConnectToPLC(string ip)
        {
            try
            {
                /*IPHostEntry iphost = Dns.GetHostEntry(ip);
                IPAddress ipadress = iphost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipadress, 502);
                ModbusClient.Connect(ipEndPoint);*/
                //ModbusClient.Close();
                //ModbusClient = new TcpClient();
                ModbusClient.Connect(ip, Port);
                ModbusClient.GetStream().ReadTimeout = 2000; //设置2秒读超时
                ModbusClient.GetStream().WriteTimeout = 2000; //设置2秒写超时
                if (ModbusClient.Connected)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                /*if (ModbusClient.Client != null)
                {
                    ModbusClient.Client.Close();
                }*/

                Console.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// 检查是否能连上IP，如是第一次连接则先连接再检查
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <returns></returns>
        public bool CheckAndConfirmToConnect(string ip)
        {
            /*if (ModbusClient == null)              //第一次连接
            {
                ModbusClient = new TcpClient();
                if (!PingTest(ip))
                {
                    return false;
                }

                return ConnectToPLC(ip);
            }
            else if (ModbusClient.Client == null)
            {
                if (!PingTest(ip))
                {
                    return false;
                }

                return ConnectToPLC(ip);
            }
            else
            {
                if (!PingTest(ip))
                {
                    return false;
                }

                if (ModbusClient.Connected)
                {
                    return true;
                }
                else
                {
                    try
                    {
                        ModbusClient.Close();
                        ModbusClient = new TcpClient();
                    }
                    catch (Exception)
                    {
                    }
                    return ConnectToPLC(ip);
                }
            }*/

            if (ModbusClient == null)
            {
                try
                {
                    ModbusClient = new TcpClient();
                    if (!PingTest(ip))
                    {
                        return false;
                    }

                    ModbusClient.Connect(ip, Port);
                    //ModbusClient.GetStream().ReadTimeout = 2000; //设置2秒读超时
                    //ModbusClient.GetStream().WriteTimeout = 2000; //设置2秒写超时
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else if (ModbusClient.Client == null)
            {
                try
                { 
                    ModbusClient = new TcpClient();
                    if (!PingTest(ip))
                    {
                        return false; ;
                    }
                    ModbusClient.Connect(ip, Port);
                    //ModbusClient.GetStream().ReadTimeout = 2000; //设置2秒读超时
                    //ModbusClient.GetStream().WriteTimeout = 2000; //设置2秒写超时
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                if (!ModbusClient.Connected)
                {
                    try
                    {
                        ModbusClient = new TcpClient();

                        if (!PingTest(ip))
                        {
                            return false; ;
                        }

                        ModbusClient.Connect(ip, Port);
                        //ModbusClient.GetStream().ReadTimeout = 2000; //设置2秒读超时
                        //ModbusClient.GetStream().WriteTimeout = 2000; //设置2秒写超时
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取传输字节总长
        /// </summary>
        /// <param name="funcode">功能码</param>
        /// <param name="sendNum">寄存器数</param>
        /// <returns>传输字节总长</returns>
        protected int GetSendbyteLength(byte funcode, int sendNum)
        {
            int length = 0;
            if (funcode == (byte)FUNCODE.读多个寄存器)
            {
                length += 7;//报文长度
                length += 5;//功能码1+起始寄存器基址2+寄存器个数2
                length += 0; //无附加数据
            }
            else if (funcode == (byte)FUNCODE.写多个寄存器)
            {
                length += 7;//报文长度
                length += 6; //功能码1+起始寄存器基址2+寄存器个数2+数据长度1
                length += (sendNum * 2);//寄存器数据
                length += 0; //无附加数据
            }
            return length;
        }

        /// <summary>
        /// 获取报文中后续字节长度值
        /// </summary>
        /// <param name="funcode">功能码</param>
        /// <param name="sendNum">寄存器个数</param>
        /// <returns>报文中后续字节长度值</returns>
        protected int GetMBAPLengthData(byte funcode, int sendNum)
        {
            int lengthData = 0;
            if (funcode == (byte)FUNCODE.读多个寄存器)
            {
                lengthData = 1 + 1 + 2 + 2; //单元标志1+功能码1+起始基址2+寄存器数2
            }
            else if (funcode == (byte)FUNCODE.写多个寄存器)
            {
                lengthData = 1 + 1 + 2 + 2 + 1 + (sendNum * 2);//单元标志1+功能码1+起始基址2+寄存器数2+数据长度1+数据（个数X2）
            }
            return lengthData;
        }

        /// <summary>
        /// 获取Modbus报文头
        /// </summary>
        /// <param name="lengthdata">报文中后续字节长度值</param>
        /// <param name="startaddress">起始地址值</param>
        /// <returns>Modbus报文头</returns>
        protected MBAPHead GetMBAPHead(int lengthdata, int startaddress, out int index)
        {
            int transH = TransferFlag / 256;
            int transL = TransferFlag % 256;
            index = TransferFlag - 1;
            if (TransferFlag > 1000)
            {
                TransferFlag = 1;
                transH = 0;
                transL = 1;
                index = 0;
            }
            TransferFlag++;
            int FollowlengthH = lengthdata / 256;
            int FollowlengthL = lengthdata % 256;
            MBAPHead mbap = new MBAPHead
            {
                TransHigh = (byte)transH,
                TransLow = (byte)transL,
                ArgreeHigh = (byte)(startaddress / 256),
                ArgreeLow = (byte)(startaddress % 256),
                LengthHigh = (byte)FollowlengthH,
                LengthLow = (byte)FollowlengthL,
                DeviceNum = 1
            };
            return mbap;
        }

        /// <summary>
        /// 解析接收到的数据，并放在接收缓冲区的数组里
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private bool ReadReciveData(byte[] buffer)
        {
            int Index = 0; //报文开始位置索引
            byte backfuncode = buffer[(int)DATAOFFSET.功能码起始];
            while (Index >= 0)
            {
                if (backfuncode == (byte)FUNCODE.读多个寄存器错误 ||
                   backfuncode == (byte)FUNCODE.写多个寄存器错误)
                {
                    int transflag = buffer[Index] * 256 + buffer[Index + 1];
                    for (int i = Index, j = 0; i < Index + 9; i++, j++)
                    {
                        ReceiveCache[transflag - 1][j] = buffer[i];
                    }
                    Index = Index + 9;//错误报文长度是9,下一个报文的索引
                    transflag = buffer[Index] * 256 + buffer[Index + 1];//下一个报文传输标识
                    if (transflag == 0)//下一个报文传输标识为0，那么数据包解析完成
                    {
                        Index = -1;
                    }
                    else
                    {
                        backfuncode = buffer[Index + (int)DATAOFFSET.功能码起始];
                    }
                }
                else if (backfuncode == (byte)FUNCODE.读多个寄存器)
                {
                    //获取寄存器的值
                    int regnum = buffer[Index + (int)DATAOFFSET.功能码起始 + 1] / 2;//传输的寄存器个数
                    int regstart = buffer[Index + (int)DATAOFFSET.协议标志起始] * 256 + buffer[Index + (int)DATAOFFSET.协议标志起始 + 1];//从协议标识位获取请求寄存器起始地址
                    int datastart = Index + 9;//buffer中，数据开始位置

                    int transflag = buffer[Index] * 256 + buffer[Index + 1];
                    for (int i = Index, j = 0; i < Index + 9 + regnum * 2; i++, j++)
                    {
                        ReceiveCache[transflag - 1][j] = buffer[i];
                    }

                    for (int i = 0; i < regnum; i++)
                    {
                        MoubusPLCReg[regstart + i] = buffer[datastart + 2 * i] * 256 + buffer[datastart + 2 * i + 1];
                    }
                    //读取下一个报文的信息
                    Index = Index + 9 + regnum * 2;//请求报文长度是9,下一个报文的索引
                    transflag = buffer[Index] * 256 + buffer[Index + 1];//下一个报文传输标识

                    if (transflag == 0)//下一个报文传输标识为0，那么数据包解析完成
                    {
                        Index = -1;
                    }
                    else
                    {
                        backfuncode = buffer[Index + (int)DATAOFFSET.功能码起始];
                    }
                }
                else if (backfuncode == (byte)FUNCODE.写多个寄存器)
                {
                    int transflag = buffer[Index] * 256 + buffer[Index + 1];
                    for (int i = Index, j = 0; i < Index + 12; i++, j++)
                    {
                        ReceiveCache[transflag - 1][j] = buffer[i];
                    }
                    Index = Index + 12;//写多个寄存器返回报文长度是12,下一个报文的索引
                    transflag = buffer[Index] * 256 + buffer[Index + 1];//下一个报文传输标识
                    if (transflag == 0)//下一个报文传输标识为0，那么数据包解析完成
                    {
                        Index = -1;
                    }
                    else
                    {
                        backfuncode = buffer[Index + (int)DATAOFFSET.功能码起始];
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 接收Modbus传输数据
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        public void ReceiveData(string ip)
        {
            if (!CheckAndConfirmToConnect(ip))
            {
                Thread.Sleep(500);
                return;
            }

            if (ModbusClient.Connected)
            {
                try
                {
                    Byte[] ReadDataMoubusByteBuffer = new Byte[recievesize];
                    ModbusClient.GetStream().Read(ReadDataMoubusByteBuffer, 0, recievesize);
                    ReadReciveData(ReadDataMoubusByteBuffer);
                }
                catch (Exception)
                {
                    ModbusClient.Close();
                    ModbusClient = null;
                }
            }
            else
            {
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Modbus写数据
        /// </summary>
        /// <param name="code">功能码</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="regNum">寄存器个数</param>
        /// <param name="index">获取的缓存区索引</param>
        /// <returns>true:成功，false:失败</returns>
        bool WriteStream(byte code, int startaddress, int regNum, out int index)
        {
            int sendbytebuffersize = GetSendbyteLength(code, regNum);
            if (sendbytebuffersize == 0)
            {
                index = -2;
                return false;
            }
            else
            {
                int Followlength = GetMBAPLengthData(code, regNum);
                byte[] sendbytebuffer = new byte[sendbytebuffersize];
                MBAPHead mbap = GetMBAPHead(Followlength, startaddress, out index);
                try
                {
                    if (code == (byte)FUNCODE.读多个寄存器)
                    {
                        sendbytebuffer[0] = mbap.TransHigh;
                        sendbytebuffer[1] = mbap.TransLow;
                        sendbytebuffer[2] = mbap.ArgreeHigh;
                        sendbytebuffer[3] = mbap.ArgreeLow;
                        sendbytebuffer[4] = mbap.LengthHigh;
                        sendbytebuffer[5] = mbap.LengthLow;
                        sendbytebuffer[6] = mbap.DeviceNum;
                        sendbytebuffer[7] = code;
                        sendbytebuffer[8] = (byte)(startaddress / 256);
                        sendbytebuffer[9] = (byte)(startaddress % 256);
                        sendbytebuffer[10] = (byte)(regNum / 256);
                        sendbytebuffer[11] = (byte)(regNum % 256);
                        lock (locker)
                        {
                            ModbusClient.GetStream().Write(sendbytebuffer, 0, sendbytebuffer.Length);
                        }
                    }
                    else if (code == (byte)FUNCODE.写多个寄存器)
                    {
                        sendbytebuffer[0] = mbap.TransHigh;
                        sendbytebuffer[1] = mbap.TransLow;
                        sendbytebuffer[2] = mbap.ArgreeHigh;
                        sendbytebuffer[3] = mbap.ArgreeLow;
                        sendbytebuffer[4] = mbap.LengthHigh;
                        sendbytebuffer[5] = mbap.LengthLow;
                        sendbytebuffer[6] = mbap.DeviceNum;
                        sendbytebuffer[7] = code;
                        sendbytebuffer[8] = (byte)(startaddress / 256);
                        sendbytebuffer[9] = (byte)(startaddress % 256);
                        sendbytebuffer[10] = (byte)(regNum / 256);
                        sendbytebuffer[11] = (byte)(regNum % 256);
                        sendbytebuffer[12] = (byte)(regNum * 2);
                        for (int i = 0; i < regNum; i++)
                        {
                            sendbytebuffer[13 + 2 * i] = (byte)(MoubusPLCReg[startaddress + i] / 256);
                            sendbytebuffer[13 + 2 * i + 1] = (byte)(MoubusPLCReg[startaddress + i] % 256);
                        }

                        lock (locker)
                        {
                            try
                            {
                                ModbusClient.GetStream().Write(sendbytebuffer, 0, sendbytebuffer.Length);
                            }
                            catch
                            { 
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ModbusClient.Close();
                    ModbusClient = null;
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 给modbus服务器发数据
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <param name="code">功能码</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="regNum">寄存器个数</param>
        /// <param name="index">获取的缓存区索引</param>
        /// <returns>true:成功，false:失败</returns>
        public bool SendData(string ip, byte code, int startaddress, int regNum, out int index)
        {
            if (!CheckAndConfirmToConnect(ip))
            {
                index = -1;
                return false;
            }
            if (WriteStream(code, startaddress, regNum, out index))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置单个指定寄存器的值
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="value">设置寄存器值</param>
        /// <returns>true:成功，false:失败</returns>
        public bool SetModbusReg(string ip, int startaddress, int value)
        {
            int index;
            bool res = false;
            MoubusPLCReg[startaddress] = value;
            bool result = SendData(ip, (byte)FUNCODE.写多个寄存器, startaddress, 1, out index);
            if (result)
            {
                if (index >= 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Thread.Sleep(150);
                        if (ReceiveCache[index][0] * 256 + ReceiveCache[index][1] == index + 1)
                        {
                            if (ReceiveCache[index][7] == (byte)FUNCODE.写多个寄存器)
                                res = true;
                            Array.Clear(ReceiveCache[index], 0, ReceiveCache[index].Length);
                            break;
                        }
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 设置从指定起始地址的一连串寄存器的值
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="value">一连串寄存器的值</param>
        /// <returns>true:成功，false:失败</returns>
        public bool SetModbusReg(string ip, int startaddress, int[] value)
        {
            int index;
            bool res = false;
            for (int j = 0; j < value.Length; j++)
            {
                MoubusPLCReg[startaddress + j] = value[j];
            }
            bool result = SendData(ip, (byte)FUNCODE.写多个寄存器, startaddress, value.Length, out index);
            if (result)
            {
                if (index >= 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Thread.Sleep(150);
                        if (ReceiveCache[index][0] * 256 + ReceiveCache[index][1] == index + 1)
                        {
                            if (ReceiveCache[index][7] == (byte)FUNCODE.写多个寄存器)
                                res = true;
                            Array.Clear(ReceiveCache[index], 0, ReceiveCache[index].Length);
                            break;
                        }
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 获取单个指定寄存器的值
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="value">获取的寄存器值</param>
        /// <returns>true:成功，false:失败</returns>
        public bool GetModbusReg(string ip, int startaddress, out int value)
        {
            int index;
            value = -1;
            bool res = false;
            bool result = SendData(ip, (byte)FUNCODE.读多个寄存器, startaddress, 1, out index);
            if (result)
            {
                if (index >= 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Thread.Sleep(150);
                        if (ReceiveCache[index][0] * 256 + ReceiveCache[index][1] == index + 1)
                        {
                            if (ReceiveCache[index][7] == (byte)FUNCODE.读多个寄存器)
                            {
                                res = true;
                                value = MoubusPLCReg[startaddress];
                            }
                            Array.Clear(ReceiveCache[index], 0, ReceiveCache[index].Length);
                            break;
                        }
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 获取从指定起始地址的一连串寄存器的值
        /// </summary>
        /// <param name="ip">连modbus服务器的IP</param>
        /// <param name="startaddress">起始地址值</param>
        /// <param name="value">获取的一连串寄存器值</param>
        /// <param name="regNum">寄存器个数</param>
        /// <returns>true:成功，false:失败</returns>
        public bool GetModbusReg(string ip, int startaddress, out int[] value, int regNum)
        {
            int index;
            value = new int[regNum];
            bool res = false;
            bool result = SendData(ip, (byte)FUNCODE.读多个寄存器, startaddress, regNum, out index);
            if (result)
            {
                if (index >= 0)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Thread.Sleep(150);
                        if (ReceiveCache[index][0] * 256 + ReceiveCache[index][1] == index + 1)
                        {
                            if (ReceiveCache[index][7] == (byte)FUNCODE.读多个寄存器)
                            {
                                res = true;
                                for (int j = 0; j < value.Length; j++)
                                {
                                    value[j] = MoubusPLCReg[startaddress + j];
                                }
                            }
                            Array.Clear(ReceiveCache[index], 0, ReceiveCache[index].Length);
                            break;
                        }
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 接收线程:专用接收返回的Modbus数据
        /// </summary>
        /// <param name="ip"></param>
        public void StartAutoReceiveData(string ip)
        {
            Task.Run(() =>
            {
                while (true)
                {
                    ReceiveData(ip);
                }
            });
        }
        /// <summary>
        /// 获取命令码1状态
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetCommand1(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, out value);
            return res;
        }
        /// <summary>
        /// MES发给PLC命令码1
        /// </summary>
        /// <param name="command">命令码1</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCCommand1(int command)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, command);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, out value);
                    if (res && value == command)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);*/
            return res;
        }

        public bool GetCommand2(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令2, out value);
            return res;
        }
        /// <summary>
        /// MES发给PLC的机床下料的仓位号
        /// </summary>
        /// <param name="rackno">下料仓位号</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCCNCBlanking(int rackno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的机床下料仓位n, rackno);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES料位机床下料仓位n, out value);
                    if (res && value == rackno)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的机床下料仓位n, 0);*/
            return res;
        }
        /// <summary>
        /// MES发给PLC的设备号
        /// </summary>
        /// <param name="deviceno">设备号</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCDevice(int deviceno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC设备号k, deviceno);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES设备号k, out value);
                    if (res && value == deviceno)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC设备号k, 0);*/
            return res;
        }
        /// <summary>
        /// MES发给PLC的机床上料的仓位号
        /// </summary>
        /// <param name="rackno">上料仓位号</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCCNCCharging(int rackno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的机床上料仓位m, rackno);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES料位机床上料仓位m, out value);
                    if (res && value == rackno)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的机床上料仓位m, 0);*/
            return res;
        }
        /// <summary>
        /// MES发给PLC的命令码2
        /// </summary>
        /// <param name="command">命令码2</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCCommand2(int command)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令2, command);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES的命令2, out value);
                    if (res && value == command)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令2, 0);*/
            return res;
        }
        /// <summary>
        /// MES发给PLC的成品底盘仓位号
        /// </summary>
        /// <param name="baseno">成品底盘仓位号</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCBaseFinished(int baseno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, baseno);
            /*if (res)
            {
                Console.WriteLine("MES发给PLC取成品底盘仓位a:{0}", baseno);
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES取成品底盘仓位a, out value);
                    Console.WriteLine("PLC响应MES取成品底盘仓位a:{0}", value);
                    if (res && value == baseno)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, 0);*/
            return res;
        }
        /// <summary>
        /// MES发给PLC的成品圆盘仓位号
        /// </summary>
        /// <param name="diskno">成品圆盘仓位号</param>
        /// <returns>成功:true，失败:false</returns>
        public bool MEStoPLCDiskFinished(int diskno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品圆盘仓位b, diskno);
            /*if (res)
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value;
                    res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES取成品圆盘仓位b, out value);
                    if (res && value == diskno)
                        break;
                    else
                        res = false;
                }
            }
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品圆盘仓位b, 0);*/
            return res;
        }
        /// <summary>
        /// 一次性发送需要装配的仓位
        /// </summary>
        /// <param name="ano">底盘号</param>
        /// <param name="bno">圆盘号</param>
        /// <returns></returns>
        public bool MEStoPLCFitFinshed(int dino, int yuanno)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;
            int[] value = new int[2];
            value[0] = dino;
            value[1] = yuanno;
            bool res1 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, value);
            /*bool res2 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品圆盘仓位b, bno);
            if (res1 && res2)
                res = true;*/
            if (res1)
                res = true;
            /*if (res1)
            {
                Thread.Sleep(650);
                int[] valuetemp = new int[2];
                bool res2 = GetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, out valuetemp, valuetemp.Length);
                if (res2 && value[0] == valuetemp[0] && value[1] == valuetemp[1])
                    res = true;
            }
            value[0] = 0;
            value[1] = 0;
            SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, value);*/
            return res;
        }

        /// <summary>
        /// MES告诉PLC使AGV去装配
        /// </summary>
        /// <returns></returns>
        public bool MEStoPLCAGVGo()
        {
            Console.WriteLine("MES告诉PLC使AGV去装配");
            bool res1 = SetModbusReg(IP, (int)DATATABLE.装配料盘出库去装配, 125);
            return res1;
        }

        /// <summary>
        /// PLC向MES发送的车床加工完成信号，202表示车床加工完成
        /// </summary>
        /// <param name="value">获取的车床加工完成信号值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESLatheProcessFinish(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送命令车床加工完, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应车床加工完, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送命令车床加工完, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应车床加工完, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res; 
        }
        /// <summary>
        /// PLC发MES的车床上料仓位号
        /// </summary>
        /// <param name="value">获取的车床上料仓位号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESLatheCharging(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的上料位值m, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应上料仓位m, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的上料位值m, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应上料仓位m, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;  
        }
        /// <summary>
        /// PLC发MES的车床下料仓位号
        /// </summary>
        /// <param name="value">获取的车床下料仓位号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESLatheBlanking(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的下料位值n, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应下料仓位n, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的下料位值n, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应下料仓位n, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;   
        }
        /// <summary>
        /// PLC发MES的车床设备号，1为车床
        /// </summary>
        /// <param name="value">设备号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESLatheDevice(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的设备k, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应设备k, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的设备k, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应设备k, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;   
        }
        /// <summary>
        /// PLC发给MES的RFID请求信号
        /// </summary>
        /// <param name="value">RFID请求信号值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESRFIDSignal(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的命令, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应PLC发送的命令, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的命令, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应PLC发送的命令, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;    
        }
        /// <summary>
        /// PLC向MES发送加工中心加工完成信号
        /// </summary>
        /// <param name="value">加工中心加工完成信号值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESCenterProcessFinish(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送命令加工中心加工完, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应加工中心加工完, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送命令加工中心加工完, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应加工中心加工完, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;   
        }
        /// <summary>
        /// PLC向MES发送加工中心上料仓位号
        /// </summary>
        /// <param name="value">加工中心上料仓位号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESCenterCharging(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的上料位值m2, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应上料仓位m2, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的上料位值m2, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应上料仓位m2, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;  
        }
        /// <summary>
        /// PLC向MES发送加工中心下料仓位号
        /// </summary>
        /// <param name="value">加工中心下料仓位号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESCenterBlanking(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的下料位值n2, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应下料仓位n2, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的下料位值n2, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应下料仓位n2, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;
        }
        /// <summary>
        /// PLC向MES发送加工中心设备号，2为加工中心
        /// </summary>
        /// <param name="value">加工中心设备号值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESCenterDevice(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的设备k2, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应设备k2, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送的设备k2, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应设备k2, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;   
        }
        /// <summary>
        /// PLC向MES发送CCD拍照结果信号
        /// </summary>
        /// <param name="value">CCD拍照结果信号值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool PLCtoMESCCDResult(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送CCD的拍照结果, out value);
            if (res)
            {
                SetModbusReg(IP, (int)DATATABLE.MES响应PLC发送CCD的拍照结果, value);
                /*for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(300);
                    int value2;
                    GetModbusReg(IP, (int)DATATABLE.PLC向MES发送CCD的拍照结果, out value2);
                    if (value2 == 0)
                    {
                        res = true;
                        SetModbusReg(IP, (int)DATATABLE.MES响应PLC发送CCD的拍照结果, 0);
                        break;
                    }
                    else
                        res = false;
                }*/
            }
            return res;  
        }
        /// <summary>
        /// 获取加工机器人的状态
        /// </summary>
        /// <param name="value">状态值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRobotStatus(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.机械手的状态, out value);
            return res;
        }
        /// <summary>
        /// 获取加工机器人的home位置与否
        /// </summary>
        /// <param name="value">home位置</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRobotHomePoint(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.机械手是否在HOME位置确认, out value);
            return res;
        }
        /// <summary>
        /// 获取加工机器人的运行模式
        /// </summary>
        /// <param name="value">运行模式</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRobotMode(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.机械手运行模式, out value);
            return res;
        }
        /// <summary>
        /// 获取加工机器人的是否繁忙信号
        /// </summary>
        /// <param name="value">繁忙信号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRobotSpeed(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.机器人繁忙, out value);
            return res;
        }
        /// <summary>
        /// 获取加工料仓的所有仓位信息
        /// </summary>
        /// <param name="value1">料仓仓位状态1</param>
        /// <param name="value2">料仓仓位状态2</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackAll(out int value1, out int value2)
        {
            value1 = 0;
            value2 = 0;
            int[] values = new int[2];
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.料仓仓位状态1, out values, values.Length);
            if (res)
            {
                value1 = values[0];
                value2 = values[1];
            }
            return res;
        }
        /// <summary>
        /// 获取加工料仓指定的仓位号状态
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="result">仓位号状态（true:有料,false:无料）</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackNO(int rackno, out bool result)
        {
            result = false;
            bool res = false;
            int value1, value2;
            if (IP == string.Empty)
                return false;

            int index = rackno - 1;
            if (index >= 0 && index < 16)
            {
                res = GetModbusReg(IP, (int)DATATABLE.料仓仓位状态1, out value1);
                if (res)
                {
                    if (index >= 8)
                    {
                        int value = value1 % 256;
                        int index2 = index - 8;
                        if ((value >> index2) % 2 == 1)
                            result = true;
                        else
                            result = false;
                    }
                    else
                    {
                        int value = value1 / 256;
                        if ((value >> index) % 2 == 1)
                            result = true;
                        else
                            result = false;
                    }
                }
            }
            else if (index >= 16 && index < 30)
            {
                res = GetModbusReg(IP, (int)DATATABLE.料仓仓位状态2, out value2);
                if (res)
                {
                    if (index >= 24)
                    {
                        int value = value2 % 256;
                        int index2 = index - 24;
                        if ((value >> index2) % 2 == 1)
                            result = true;
                        else
                            result = false;
                    }
                    else
                    {
                        int value = value2 / 256;
                        int index2 = index - 16;
                        if ((value >> index2) % 2 == 1)
                            result = true;
                        else
                            result = false;
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// 获取加工料仓PLC里存储的指定仓位号的信息
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="values">存储信息的数组</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackPLCInfo(int rackno, out int[] values)
        {
            values = new int[4];
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.仓位1场次 + (rackno - 1) * 4, out values, values.Length);
            return res;
        }
        /// <summary>
        ///  获取加工料仓PLC里存储的指定仓位号的场次
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="value">获取的场次</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackPLCScene(int rackno, out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.仓位1场次 + (rackno - 1) * 4, out value);
            return res;
        }
        /// <summary>
        /// 获取加工料仓PLC里存储的指定仓位号的编号
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="value">获取的编号</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackPLCNumber(int rackno, out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.仓位1编号 + (rackno - 1) * 4, out value);
            return res;
        }
        /// <summary>
        /// 获取加工料仓PLC里存储的指定仓位号的零件类型
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="value">获取的零件类型</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackPLCType(int rackno, out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.仓位1零件类型 + (rackno - 1) * 4, out value);
            return res;
        }
        /// <summary>
        /// 获取加工料仓PLC里存储的指定仓位号的零件状态
        /// </summary>
        /// <param name="rackno">指定仓位号</param>
        /// <param name="value">获取的零件状态</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetProcessRackPLCState(int rackno, out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.仓位1零件状态 + (rackno - 1) * 4, out value);
            return res;
        }

        public bool SetProcessRackPLCState(int rackno, int value)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.仓位1零件状态 + (rackno - 1) * 4, value);
            return res;
        }

        public bool SetProcessRackPLCType(int rackno, int value)
        {
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = SetModbusReg(IP, (int)DATATABLE.仓位1零件类型 + (rackno - 1) * 4, value);
            return res;
        }

        /// <summary>
        /// 获取装配料仓的所有仓位信息
        /// </summary>
        /// <param name="value1">装配仓位状态1</param>
        /// <param name="value2">装配仓位状态2</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetFitRackAll(out int value1, out int value2)
        {
            value1 = 0;
            value2 = 0;
            int[] values = new int[2];
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.装配料仓仓位状态1, out values, values.Length);
            if (res)
            {
                value1 = values[0];
                value2 = values[1];
            }
            return res;
        }

        /// <summary>
        /// 获取中转台光电信息
        /// </summary>
        /// <param name="value">光电的寄存器值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetStationInfo(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.中转台工位光电, out value);
            return res;
        }
        /// <summary>
        ///  获取指定中转台光电状态
        /// </summary>
        /// <param name="stationno">中转台号</param>
        /// <param name="states">中转台光电状态值</param>
        /// <returns>成功:true表示成功获取到信号值，失败:false</returns>
        public bool GetTheStationSensor(int stationno, out bool[] states)
        {
            int value;
            states = new bool[4];
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.中转台工位光电, out value);
            if (res)
            {
                int valuetemp = value / (2 * 2 * 2 * 2 * 2 * 2 * 2 * 2);  //由于与PLC的高低字节不一样故做处理
                for (int i = 0; i < 4; i++)
                {
                    int index = (stationno == 1) ? i : (i + 4);
                    if ((valuetemp >> (i)) % 2 == 1)
                        states[i] = false;
                    else
                        states[i] = true;
                }
            }
            return res;
        }

        /// <summary>
        /// 获取AGV位置信号，值的第0位表示AGV到站台1，第1位表示AGV到站台2，第2位表示AGV上有料盘
        /// </summary>
        /// <param name="value">寄存器的值</param>
        /// <returns></returns>
        public bool GetAGVPositionState(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.AGV与站台信号, out value);
            return res;
        }

        public static bool plcgetconfim = false;
        public void CheckMESToPLCConfirmSl()
        {
            int value1, value2;
            bool res1 = GetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, out value1);
            bool res2 = GetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, out value2);
            if (res1 && res2)
            {
                if (LineMainForm.MES_PLC_comfim_flage)
                {
                    LineMainForm.MES_PLC_comfim_count++;
                    if (LineMainForm.MES_PLC_comfim_count > 60)//指令下达交互时间1min无回应
                    {
                        string temp;
                        if (value1 == (int)COMMAND1.启动系统)
                        {
                            temp = "启动超时,请复位PLC！";

                            LineMainForm.linestart = true;//切换产线复位按钮
                            LineMainForm.linestarting = false;

                            LineMainForm.MES_PLC_comfim_count = 0;
                            LineMainForm.MES_PLC_comfim_flage = false;
                            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                            SetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, 0);
                            plcgetconfim = false;
                            MessageBox.Show(temp);
                            LineMainForm.Linebuttomcur = 2;
                            return;
                        }
                        else if (value1 == (int)COMMAND1.停止系统)
                        {
                            temp = "停止超时,请复位PLC！";

                            LineMainForm.linestop = true;//切换产线复位按钮
                            LineMainForm.linestoping = false;

                            LineMainForm.MES_PLC_comfim_count = 0;
                            LineMainForm.MES_PLC_comfim_flage = false;
                            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                            SetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, 0);
                            plcgetconfim = false;
                            LineMainForm.Linebuttomcur = 2;
                            MessageBox.Show(temp);
                            return;
                        }
                        else if (value1 == (int)COMMAND1.启动设备)
                        {
                            temp = "复位超时,请复位PLC！";

                            LineMainForm.linereset = false;//切换产线复位按钮
                            LineMainForm.linereseting = false;

                            LineMainForm.MES_PLC_comfim_count = 0;
                            LineMainForm.MES_PLC_comfim_flage = false;
                            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                            SetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, 0);
                            plcgetconfim = false;
                            MessageBox.Show(temp);
                            LineMainForm.Linebuttomcur = 2;
                            return;
                        }
                        else if (value1 == (int)COMMAND1.加工调度)
                        {
                            LineMainForm.MES_PLC_comfim_count = 0;
                            LineMainForm.MES_PLC_comfim_flage = false;
                            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                            SetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, 0);
                            plcgetconfim = false;
                            return;
                        }
                        else
                        {
                            LineMainForm.MES_PLC_comfim_count = 0;
                            LineMainForm.MES_PLC_comfim_flage = false;
                            SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                            SetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, 0);
                            plcgetconfim = false;
                            return;
                        }
                    }
                }

                if (value1 == value2 && value2 != 0)
                {
                    if (value2 == (int)COMMAND1.启动系统)
                        Console.WriteLine("产线启动响应");
                    else if (value2 == (int)COMMAND1.停止系统)
                        Console.WriteLine("产线停止响应");
                    else if (value2 == (int)COMMAND1.启动设备)
                        Console.WriteLine("产线复位响应");
                    else if (value2 == (int)COMMAND1.加工调度)
                        Console.WriteLine("加工调度响应");
                    SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                    plcgetconfim = true;
                }

                if (LineMainForm.MES_PLC_comfim_flage && plcgetconfim && value1 == 0 && value2 == 0)
                {
                    plcgetconfim = false;
                    LineMainForm.MES_PLC_comfim_count = 0;
                    LineMainForm.MES_PLC_comfim_flage = false;
                }
            }
        }

        /// <summary>
        /// 检测PLC是否响应命令来清除MES发PLC信号
        /// </summary>
        public void CheckToCleanMEStoPLCSingal()
        {
            int valuetemp;
            bool res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES命令, out valuetemp);
            if (res && valuetemp == 120)
            {
                Console.WriteLine("清空装配料号"); 
                bool res1 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                bool res2 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, 0);
                bool res3 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品圆盘仓位b, 0);
                Thread.Sleep(100);
                Console.WriteLine("{0}, {1}, {2}", res1, res2, res3);
                res1 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC的命令, 0);
                res2 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品底盘仓位a, 0);
                res3 = SetModbusReg(IP, (int)DATATABLE.MES发给PLC取成品圆盘仓位b, 0);
                Console.WriteLine("{0}, {1}, {2}", res1, res2, res3);
            }
            int[] value = new int[10];
            res = GetModbusReg(IP, (int)DATATABLE.PLC响应MES料位机床下料仓位n, out value, value.Length);
            if (res)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == 0)
                        continue;
                    SetModbusReg(IP, (int)DATATABLE.MES发给PLC的机床下料仓位n + i, 0);
                }
            }
        }

        public void CheckToCleanPLCToMESSingal()
        {
            int[] value = new int[10];
            bool res = GetModbusReg(IP, (int)DATATABLE.PLC向MES发送命令车床加工完, out value, value.Length);
            if (res)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == 0)
                        SetModbusReg(IP, (int)DATATABLE.MES响应车床加工完 + i, 0);
                }
            }
        }

        public bool GetLatheProcessed(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.车床加工完成状态, out value);
            return res;
        }

        public bool GetCenterProcessed(out int value)
        {
            value = 0;
            bool res = false;
            if (IP == string.Empty)
                return false;

            res = GetModbusReg(IP, (int)DATATABLE.加工中心加工完成状态, out value);
            return res;
        }
    }
}
