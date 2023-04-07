using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCADA
{
    /// <summary>
    /// 串口通信状态
    /// </summary>
    public enum SpCommStates { 通信异常 = -1, 初始化 = 0, 串口未开启 = 1, 通信正常 = 2, 串口已关闭 = 3 };

    public class SpCommStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 串口通信状态
        /// </summary>
        public SpCommStates State { get; private set; }

        /// <summary>
        /// 串口通信状态说明
        /// </summary>
        public string StateText { get; private set; }

        public SpCommStateChangedEventArgs(SpCommStates spCommState)
        {
            State = spCommState;
            StateText = Enum.GetName(typeof(SpCommStates), spCommState);
        }
    }

    public class SerialPortCommBase : IDisposable
    {
        private SerialPort sp = new SerialPort();
        private Object thisLock = new Object();
        private Object thisLock2 = new Object();

        /// <summary>
        /// 获取一个值，该值指示 SerialPortCommBase 对象的打开或关闭状态
        /// </summary>
        public bool IsOpen { get { return sp.IsOpen; } }

        /// <summary>
        /// 当串口通信状态改变时发生
        /// </summary>
        public event EventHandler<SpCommStateChangedEventArgs> SpCommStateChanged;

        /// <summary>
        /// 触发SpCommStateChanged事件
        /// </summary>
        /// <param name="state"></param>
        private void OnSpCommStateChanged(SpCommStates state)
        {
            if (SpCommStateChanged != null)
            {
                SpCommStateChanged(this, new SpCommStateChangedEventArgs(state));
            }
        }

        private SpCommStates _spCommState;
        /// <summary>
        /// 串口通信状态
        /// </summary>
        public SpCommStates SpCommState
        {
            get
            {
                return _spCommState;
            }
            private set
            {
                if (_spCommState != value)
                {
                    _spCommState = value;
                    OnSpCommStateChanged(_spCommState);
                }
            }
        }

        /// <summary>
        /// 初始化 串口通信基础（SerialPortCommBase） 类的新实例
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public SerialPortCommBase(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            sp.PortName = portName;
            sp.BaudRate = baudRate;
            sp.Parity = parity;
            sp.DataBits = dataBits;
            sp.StopBits = stopBits;
            sp.Encoding = Encoding.ASCII;
            sp.NewLine = Environment.NewLine;
            SpCommState = SpCommStates.串口未开启;

        }

        /// <summary>
        /// 打开串行端口连接
        /// </summary>
        public bool Open()
        {
            if (!sp.IsOpen)
            {
                sp.DtrEnable = true;
                sp.RtsEnable = true;
                sp.ReadTimeout = 1000;
                try
                {
                    sp.Open();

                    if (sp.IsOpen)
                    {
                        SpCommState = SpCommStates.通信正常;

                        Console.WriteLine("串口{0}打开成功", sp.PortName);
                        return true;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(string.Format("未知错误，串口{0}打开失败！", sp.PortName));
                        return false;
                    }

                }

                catch (Exception ex)
                {
                    SpCommState = SpCommStates.通信异常;
                    return false;
                    //throw ex;
                }
            }
            else return true;
        }

        /// <summary>
        /// 关闭串行端口连接
        /// </summary>
        public virtual void Close()
        {
            try
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                if (!sp.IsOpen)
                {
                    SpCommState = SpCommStates.串口已关闭;
                }
                else
                {
                    throw new UnauthorizedAccessException(string.Format("未知错误，串口{0}关闭失败！", sp.PortName));
                }
            }
            catch (Exception ex)
            {
                SpCommState = SpCommStates.通信异常;
                //throw ex;
            }
        }

        /// <summary>
        /// 发送命令获取结果
        /// </summary>
        /// <param name="cmdText">命令</param>
        /// <returns>返回的结果</returns>
        protected string SendCmd(string cmdText)
        {
            string result = string.Empty;
            if (sp.IsOpen)
            {
                sp.WriteLine(cmdText);
                Thread.Sleep(1000);
                result = sp.ReadExisting();
            }
            return result;
        }

        /// <summary>
        /// 生成CRC校验码
        /// </summary>
        /// <param name="message"></param>
        /// <param name="CRC"></param>
        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            ushort CRCFull = 0xFFFF;
            byte CRCHigh = 0xFF, CRCLow = 0xFF;
            char CRCLSB;
            for (int i = 0; i < message.Length - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);
                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    //下面两句所得结果一样
                    //CRCFull = (ushort)(CRCFull >> 1);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);
                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }
            CRC[1] = CRCHigh = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = CRCLow = (byte)(CRCFull & 0xFF);
        }

        /// <summary>
        /// 生成主机发送命令
        /// （功能码为03H（只读）时，生成全部命令；
        /// 功能码为10H（写）时，生成部分命令（未生成写入的数据）；
        /// 功能码为01H时，生成全部命令）
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="start">起始地址（寄存器地址）</param>
        /// <param name="registers">数据长度（寄存器数量）</param>
        /// <param name="message">存放生成的主机发送命令</param>
        private void BuildMessage(byte address, byte functionCode, ushort start, ushort registers, ref byte[] message)
        {
            //用于存放生成的CRC码
            byte[] CRC = new byte[2];
            //生成主机发送命令
            message[0] = address;
            message[1] = functionCode;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;
            //根据命令生成CRC码
            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];//低8位
            message[message.Length - 1] = CRC[1];//高8位
        }

        /// <summary>
        /// 生成主机发送命令（功能码为05（写）时，生成部分命令（未生成写入的数据））
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="functionCode">功能码</param>
        /// <param name="coilAddress">位地址（线圈地址）</param>
        /// <param name="message">存放生成的主机发送命令</param>
        private void BuildMessage05(byte address, byte functionCode, ushort coilAddress, ref byte[] message)
        {
            //用于存放生成的CRC码
            byte[] CRC = new byte[2];
            //生成主机发送命令
            message[0] = address;
            message[1] = functionCode;
            message[2] = (byte)(coilAddress >> 8);
            message[3] = (byte)coilAddress;
            //根据命令生成CRC码
            GetCRC(message, ref CRC);
            message[message.Length - 2] = CRC[0];//低8位
            message[message.Length - 1] = CRC[1];//高8位
        }

        /// <summary>
        /// 获取从机返回的命令
        /// </summary>
        /// <param name="response">用于存放从机返回的命令</param>
        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.

            try
            {
                int length = response.Length;
                for (int i = 0; i < length; i++)
                {
                    int t = sp.ReadByte();
                    if (t == -1)
                    {
                        return;
                    }
                    response[i] = (byte)t;
                    if (i == 1)
                    {
                        if (response[1] > 0x80)
                            length = 5;
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                //throw;
            }
        }

        /// <summary>
        /// 判断从机返回的命令是否正确
        /// </summary>
        /// <param name="response">从机返回的命令</param>
        /// <returns></returns>
        private bool CheckResponse(byte[] response)
        {
            byte[] CRC = new byte[2];
            GetCRC(response, ref CRC);
            if (CRC[0] == response[response.Length - 2] && CRC[1] == response[response.Length - 1])
                return true;
            else
                return false;
        }

        /// <summary>
        /// 判断从机响应不正常原因
        /// </summary>
        /// <param name="AbnormalCode">不正常代码</param>
        /// <param name="result">不正常原因</param>
        private void AbnormalResult(byte AbnormalCode, ref string result)
        {
            byte Code = (byte)(AbnormalCode ^ 0x80);
            switch (Code)
            {
                case 0x01: result = "不合法功能代码"; break;
                case 0x02: result = "不合法数据地址"; break;
                case 0x03: result = "不合法数据"; break;
                case 0x04: result = "从机设备故障"; break;
                case 0x05: result = "确认"; break;
                case 0x06: result = "从机设备忙碌"; break;
                case 0x07: result = "否定"; break;
                case 0x08: result = "内存奇偶校验错误"; break;
                default: result = "其它"; break;
            }
        }

        /// <summary>
        /// 往COM口写数据，并返回结果
        /// </summary>
        /// <param name="message">要写入的数据</param>
        /// <param name="response">返回的命令</param>
        /// <returns></returns>
        private bool WriteCOM(byte[] message, ref byte[] response)
        {
            //lock 确保当一个线程位于代码的临界区时，另一个线程不进入临界区。如果其他线程试图进入锁定的代码，
            //则它将一直等待（即被阻止），直到该对象被释放。
            lock (thisLock2)
            {
                sp.Write(message, 0, message.Length);
                GetResponse(ref response);
                return true;
            }
        }

        /// <summary>
        /// 主机发送功能码为03H的命令，并获取从机返回的命令
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="start">起始地址（寄存器地址）</param>
        /// <param name="registers">数据长度（寄存器数量）</param>
        /// <param name="returnValues">从机返回的查询结果</param>
        /// <param name="result">从机响应结果是否正常，正常时返回“e正常”，不正常是否返回错误原因</param>
        /// <returns></returns>
        protected bool SendFc03(byte address, ushort start, ushort registers, ref ushort[] returnValues, ref string result)
        {
            lock (thisLock)
            {
                if (sp.IsOpen)
                {
                    sp.DiscardOutBuffer();//丢弃来自串行驱动程序的传输缓冲区的数据
                    sp.DiscardInBuffer();//丢弃来自串行驱动程序的接收缓冲区的数据
                    //功能码为03H时，主机发送的命令为8 bytes
                    byte[] message = new byte[8];

                    //功能码为03H时，从机返回的命令长度为5+2*查询寄存器数量
                    byte[] response = new byte[5 + 2 * registers];
                    BuildMessage(address, 0x03, start, registers, ref message);

                    if (!WriteCOM(message, ref response))
                    {
                        result = "往从机发送命令出错；";
                        return false;
                    }

                    if (CheckResponse(response))
                    {
                        if (response[1] > 0x80)//属于不正常响应//
                        {
                            AbnormalResult((byte)response[2], ref result);
                            return false;
                        }
                        else
                        {
                            result = "正常";
                            for (int i = 0; i < response.Length - 5; i++)
                            {
                                returnValues[i] = response[i + 3];
                                //returnValues[i] <<= 8;
                                //returnValues[i] += response[i + 4];
                            }
                            return true;
                        }
                    }
                    else//CRC错误
                    {
                        result = "CRC错误";
                        return false;
                    }
                }
                else//COM口未打开
                {
                    result = "COM口未打开";
                    return false;
                }
            }
        }

        /// <summary>
        /// 主机发送功能码为01H的命令，并获取从机返回的命令
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="start">起始地址（寄存器地址）</param>
        /// <param name="registers">数据长度（寄存器数量）</param>
        /// <param name="returnValues">从机返回的查询结果</param>
        /// <param name="result">从机响应结果是否正常，正常时返回“正常”，不正常是否返回错误原因</param>
        /// <returns></returns>
        protected bool SendFc01(byte address, ushort start, ushort registers, ref char[] returnValues, ref string result)
        {
            if (sp.IsOpen)
            {
                //功能码为01H时，主机发送的命令为8 bytes
                byte[] message = new byte[8];
                //计数位（8位1个）
                int DataCoils = Convert.ToInt32(registers) / 8 + 1;
                byte[] response = new byte[5 + DataCoils];
                BuildMessage(address, 0x01, start, registers, ref message);
                if (!WriteCOM(message, ref response))
                {
                    result = "往从机发送命令出错；";
                    return false;
                }
                if (CheckResponse(response))
                {
                    if (response[1] > 0x80)//属于不正常响应
                    {
                        AbnormalResult((byte)response[2], ref result);
                        return false;
                    }
                    else
                    {
                        result = "正常";
                        int j = 0;
                        for (int i = 0; i < response.Length - 5; i++)
                        {
                            byte data = response[i + 3];
                            for (int n = 0; n < 8; n++)
                            {
                                returnValues[j] = (char)(data & 0x0001);
                                data = (byte)(data >> 1);
                                j++;
                                if (j >= returnValues.Length)
                                {
                                    break;
                                }
                            }
                        }
                        return true;
                    }
                }
                else//CRC错误
                {
                    result = "CRC错误";
                    return false;
                }
            }
            else//COM口未打开
            {
                result = "COM口未打开";
                return false;
            }
        }

        /// <summary>
        /// 主机发送功能码为10H的命令，并获取从机返回的命令
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="start">起始地址（寄存器地址）</param>
        /// <param name="registers">数据长度（寄存器数量）</param>
        /// <param name="values">需写入的数据</param>
        /// <param name="result">从机响应结果是否正常，正常时返回“正常”，不正常是否返回错误原因</param>
        /// <returns></returns>
        protected bool SendFc10(byte address, ushort start, ushort registers, ushort[] values, ref string result)
        {
            lock (thisLock)
            {
                if (sp.IsOpen)
                {
                    //功能码为10H时，主机发送的命令为9+2*写寄存器数量
                    byte[] message = new byte[9 + 2 * registers];
                    //功能码为10H时，从机返回的命令为8bytes
                    byte[] response = new byte[8];
                    //字节计数，寄存器数量*2
                    message[6] = (byte)(registers * 2);
                    //保存的数据（需写入的数据）
                    for (int i = 0; i < registers; i++)
                    {
                        message[7 + 2 * i] = (byte)(values[i] >> 8);
                        message[8 + 2 * i] = (byte)(values[i]);
                    }
                    BuildMessage(address, 0x10, start, registers, ref message);
                    if (!WriteCOM(message, ref response))
                    {
                        result = "往从机发送命令出错；";
                        return false;
                    }
                    if (CheckResponse(response))
                    {
                        if (response[1] > 0x80)//属于不正常响应
                        {
                            AbnormalResult((byte)response[2], ref result);
                            return false;
                        }
                        else
                        {
                            result = "正常";
                            return true;
                        }
                    }
                    else//CRC错误
                    {
                        result = "CRC错误";
                        return false;
                    }
                }
                else//COM口未打开
                {
                    result = "COM口未打开";
                    return false;
                }
            }
        }

        /// <summary>
        /// 主机发送功能码为05H的命令，并获取从机返回的命令
        /// </summary>
        /// <param name="address">设备地址</param>
        /// <param name="start">起始地址（寄存器地址）</param>
        /// <param name="values">需写入的数据</param>
        /// <param name="result">从机响应结果是否正常，正常时返回“正常”，不正常是否返回错误原因</param>
        /// <returns></returns>
        protected bool SendFc05(byte address, ushort start, ushort values, ref string result)
        {
            if (sp.IsOpen)
            {
                //功能码为05H时，主机发送的命令为8bytes
                byte[] message = new byte[8];
                ////功能码为05H时，从机返回的命令为8bytes
                byte[] response = new byte[8];
                message[4] = (byte)(values >> 8);
                message[5] = (byte)(values);
                BuildMessage05(address, 0x05, start, ref message);
                if (!WriteCOM(message, ref response))
                {
                    result = "往从机发送命令出错；";
                    return false;
                }

                if (CheckResponse(response))
                {
                    if (response[1] > 0x80)//属于不正常响应
                    {
                        AbnormalResult((byte)response[2], ref result);
                        return false;
                    }
                    else
                    {
                        bool same = true;
                        for (int i = 0; i < message.Length; i++)
                        {
                            if (message[i] != response[i])
                            {
                                same = false;
                                break;
                            }
                        }
                        if (same)
                        {
                            result = "正常";
                            return true;
                        }
                        else
                        {
                            result = "从机返回命令与主机发送命令不一致";
                            return false;
                        }
                    }
                }
                else//CRC错误
                {
                    result = "CRC错误";
                    return false;
                }
            }
            else//COM口未打开
            {
                result = "COM口未打开";
                return false;
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    //TODO清理托管资源
                    sp.Dispose();
                }
                //TODO清理非托管资源
                isDisposed = true;
            }
        }

        ~SerialPortCommBase()
        {
            Dispose(false);
        }
    }
}
