using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyModbus;

namespace SCADA.SimensPLC
{
    public class ModbusTCPClient : ModbusClient
    {
        private bool _connected = false;
        private object locker = new object();
        public string IP;
        public int Port;

        public bool GetOnlineState()
        {
            if (_connected && Connected)
                return true;
            else
                return false;
        }

        public ModbusTCPClient()
        {
            UDPFlag = false;
        }

        public bool Connecting(string ipAddress, int port)
        {
            bool res = false;
            try
            {
                Connect(ipAddress, port);
                IP = ipAddress;
                Port = port;
                _connected = true;
                res = true;
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + "      " + DateTime.Now);
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + "      " + DateTime.Now);
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + "      " + DateTime.Now);
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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString() + "      " + DateTime.Now);
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool ReadSingleCoil(int startaddress, out bool value)
        {
            bool res = false;
            value = false;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        bool[] values = ReadCoils(startaddress, 1);
                        value = values[0];
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool ReadMutiCoil(int startaddress, int cout, out bool[] values)
        {
            bool res = false;
            values = null;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        values = ReadCoils(startaddress, cout);
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        _connected = false;
                    }
                }
            }
            return res;
        }

        public bool WritesingleCoil(int startaddress, bool value)
        {
            bool res = false;
            if (Connected && _connected)
            {
                lock (locker)
                {
                    try
                    {
                        WriteSingleCoil(startaddress, value);
                        res = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
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
                temp = value & (1 << bit);
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
