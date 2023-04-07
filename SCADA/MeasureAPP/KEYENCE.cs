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
    /// 基恩士测量仪
    /// </summary>
    public sealed class KEYENCE : SerialPortCommBase
    {
        /// <summary>
        /// 获取测量结果的命令
        /// </summary>
        private string resultCmd = "M1,0";

        /// <summary>
        /// Reset
        /// </summary>
        private string resetCmd = "Q0";

        /// <summary>
        /// 初始化 基恩士测量仪（KEYENCE） 类的新实例
        /// </summary>
        /// <param name="getResultCmd">获取测量结果的命令，默认值为M1,0</param>
        public KEYENCE(string getResultCmd = "M1,0", string portName = "COM1", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            this.resultCmd = getResultCmd;
        }

        /// <summary>
        /// 测量结果
        /// </summary>
        /// <returns></returns>
        public string GetMeasureResult()
        {
            string result = SendCmd(resultCmd);
            return result;
        }

        /// <summary>
        /// Reset
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            SendCmd(resetCmd);
            return true;
        }

    }
}
