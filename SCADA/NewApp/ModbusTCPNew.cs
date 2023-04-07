using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using EasyModbus;

namespace SCADA
{
    public class ModbusTCPNew : ModbusClient
    {
        private bool _connected = false;
        private object locker = new object();

        public bool GetOnlineState()
        {
            return _connected;
        }

        public ModbusTCPNew()
        {
            UDPFlag = false;
        }

        public bool Connecting(string ipAddress, int port)
        {
            bool res = false;
            try
            {
                Connect(ipAddress, port);
                _connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _connected = false;
            }
            return res;
        }

        public bool ReadMutiRegisters(int startingAddress, int quantity, out int[] values)
        {
            bool res = false;
            values = new int[quantity];
            if (Connected && _connected && quantity > 0)
            {
                lock (locker)
                {
                    try
                    {
                        values = ReadHoldingRegisters(startingAddress, quantity);
                        res = true;
                    }
                    catch (Exception)
                    {
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool ReadsingleRegister(int startingAddress, out int value)
        {
            bool res = false;
            int[] values;
            value = 0;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        values = ReadHoldingRegisters(startingAddress, 1);
                        value = values[0];
                        res = true;
                    }
                    catch (Exception)
                    {
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool WriteMultiRegisters(int startingAddress, int[] values)
        {
            bool res = false;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        WriteMultipleRegisters(startingAddress, values);
                        res = true;
                    }
                    catch (Exception)
                    {
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool WritesingleRegister(int startingAddress, int value)
        {
            bool res = false;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        WriteSingleRegister(startingAddress, value);
                        res = true;
                    }
                    catch (Exception)
                    {
                        _connected = false;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取点位信息
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="bit">点位</param>
        /// <returns></returns>
        public static bool GetBoolValue(int value, int bit)
        {
            int temp = 0;
            if (bit < 0 || bit > 31)
            {
                return false;
            }
            else
            {
                temp = value & (1 >> bit);
                return temp > 0;
            }
        }

        /// <summary>
        /// 设置点位信息
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="bit">点位</param>
        public static int SetBoolValue(int value, int bit)
        {
            int temp = 0;
            if (bit < 0 || bit > 31)
            {
                return 0;
            }
            else
            {
                temp = value | ((1 << bit));
                return temp;
            }
        }

        /// <summary>
        /// 清除点位信息
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bit"></param>
        public static int ClrBoolValue(int value, int bit)
        {
            int temp;
            if (bit < 0 || bit > 31)
            {
                return 0;
            }
            else
            {
                temp = value & (~(1 << bit));
                return temp;
            }
        }
    }
}
