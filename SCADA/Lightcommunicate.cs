using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collector;
using HNC_MacDataService;
using HNCAPI;
using System.IO.Ports;

namespace SCADA
{
   
          /// <summary>
    /// 串口通信状态
    /// </summary>


    public class lightcommunicate : IDisposable
    {
       public SerialPort Lightsp = new SerialPort();
        private static Object thisLock = new Object();
        private static int messagecount = 0;//发送报文次数计数，作为校验码
        public static int  portstate ;
        public  bool  portisopen = false;
        public enum LightColor
        {
            light_red = 1,//红色
            light_green,       //   绿色
            light_yellow,//黄色
            light_blue,//蓝色
            light_purple,//紫色
            light_cyan,//青色
            light_write,//白色
        };
        public enum com_state
        {
            comclose = 1,//串口关闭
            comopen = 2,//串口打开
            comnormal = 3,//串口通信正常
            comabnormal = 4,//串口通信异常         
            comopenerr= 5,//串口打开异常          
            comcloseerr= 6,//串口关闭异常
        }

    
        /// <summary>
        /// 初始化 串口通信基础（SerialPortCommBase） 类的新实例
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        public lightcommunicate()
        {
            Lightsp.PortName = "COM3";
            Lightsp.BaudRate =115200;
            Lightsp.Parity = Parity.Even;
            Lightsp.DataBits = 8;
            Lightsp.StopBits = StopBits.One;
            portstate = (int)com_state.comclose;
        }

        /// <summary>
        /// 打开串行端口连接
        /// </summary>
        public bool  Open()
        {
            if (!Lightsp.IsOpen)
            {
                 Lightsp.PortName = "COM3";
                Lightsp.BaudRate =115200;
                Lightsp.Parity = Parity.Even;
                Lightsp.DataBits = 8;
                Lightsp.StopBits = StopBits.One;
                //Lightsp.DtrEnable = true;//启用控制终端就续信号
                //Lightsp.RtsEnable = true; //启用请求发送信号
                //Lightsp.ReadTimeout = 1000;
                //Lightsp.WriteTimeout = 1000;
                try
                {
                    //Lightsp.DtrEnable = true;
                    //Lightsp.RtsEnable = true;
                    Lightsp.Open();

                    if (Lightsp.IsOpen)
                    {
                        portisopen= true;
                        portstate =(int) com_state.comnormal;
                        return true;
                    }
                    else
                    {
                        portisopen= false;
                        portstate =(int)com_state.comopenerr;
                        return  false;
                    }
                }
                catch (Exception ex)
                {
                    portisopen= false;
                    portstate =(int) com_state.comabnormal;
                    return false;
                }
            }
            return true;
            
        }

        /// <summary>
        /// 关闭串行端口连接
        /// </summary>
        public  bool Close()
        {
            try
            {
                if (Lightsp.IsOpen)
                {
                    portisopen = false;
                    portstate = (int)com_state.comclose;
                    Lightsp.Close();
                    return true;
                }
                if (!Lightsp.IsOpen)
                {
                    
                    portisopen= false;
                    portstate = (int)com_state.comclose;
                    return true;
                }
                else
                {
                    portstate = (int)com_state.comcloseerr;
                    return false;
                }
            }
            catch (Exception ex)
            {
                portstate = (int)com_state.comabnormal;
                return false;
                //throw ex;
            }
        }

/// <summary>
/// 发送五色灯信息
/// </summary>
/// <param name="color"></param>
/// <param name="no"></param>
/// <returns></returns>
       public bool SendMessage(byte color,byte no)
        {
            if (Lightsp.IsOpen)
            {
                byte[] message=new byte[10];
                for(int i = 0;i<10;i++)
                {
                    message[i]= 0;
                }
                BuildMessage(color,no,ref message);
                WriteCOM(message);
                Thread.Sleep(100);               
                return true;
            } 
            return false;
        }


  /// <summary>
  /// 报文
  /// </summary>
  /// <param name="color">灯的颜色</param>
  /// <param name="no">灯的编号</param>
  /// <param name="message">报文内容</param>
        private void BuildMessage(byte color,byte no, ref byte[] message)
        {
            
            messagecount++;
            if(messagecount>10000)
            {
                messagecount = 0;
            }

            //用于存放生成的CRC码
            int noint = (int)no;
            //生成主机发送命令
            //帧头三个字节
            message[0] = 0x01;
            message[1] =0x10;
            message[2] = 0x03;
            //LED编号两个字节
            message[3] = 0;
            message[4] =(byte)(( no/16)*16+no%16);
            //等的颜色一个字节
            message[5] =color ;

            //校验码，随机两个字节
           message[6] =(byte)(messagecount/256 );
           message[7] = (byte)(messagecount %256);
            //结束码0A
            message[8] =0x0A ;
        }


        /// <summary>
        /// 往COM口写数据，并返回结果
        /// </summary>
        /// <param name="message">要写入的数据</param>
        /// <param name="response">返回的命令</param>
        /// <returns></returns>
        private bool WriteCOM(byte[] message)
        {
            //lock 确保当一个线程位于代码的临界区时，另一个线程不进入临界区。如果其他线程试图进入锁定的代码，
            //则它将一直等待（即被阻止），直到该对象被释放。
            lock (thisLock)
            {
                try
                {
                    Lightsp.Write(message, 0, message.Length);
                    return true;
                }
                catch
                {
                    return false;
                }
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
                    Lightsp.Dispose();
                }
                //TODO清理非托管资源
                isDisposed = true;
            }
        }

        ~lightcommunicate()
        {
            Dispose(false);
        }
    }
}

