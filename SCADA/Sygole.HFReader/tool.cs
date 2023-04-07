﻿#define DEBUG_L0_ENABLE
#define DEBUG_L1_ENABLE
//#define DEBUG_L2_ENABLE

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Sygole.HFReader
{
    class tool
    {
        /// <summary>
        /// 获取BCC校验
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte GetBCC(byte[] data, int offset, int len)
        {
            byte BCC = 0;

            for (int i = 0; i < len; i++)
            {
                BCC ^= data[i + offset];
            }

            return BCC;
        }


        static UInt16[] wCRCTalbeAbs = new UInt16[]{
	        0x0000, 0xCC01, 0xD801, 0x1400, 
	        0xF001, 0x3C00, 0x2800, 0xE401, 
	        0xA001, 0x6C00, 0x7800, 0xB401, 
	        0x5000, 0x9C01, 0x8801, 0x4400, 
        };

        public static UInt16 GetCRC16(byte[] data, int offset, int len)
        {
            UInt16 wCRC = 0xFFFF;
            int i = 0;
            byte chChar = 0x00;

            for (i = offset; i < len; i++)
            {
                chChar = data[i];
                wCRC = (ushort)(wCRCTalbeAbs[(chChar ^ wCRC) & 15] ^ (wCRC >> 4));
                wCRC = (ushort)(wCRCTalbeAbs[((chChar >> 4) ^ wCRC) & 15] ^ (wCRC >> 4));
            }

            return wCRC;
        }


        public static void setBit16(ref UInt16 reg, int bit, bool value)
        {
            if (value == true)
            {
                reg = (UInt16)(reg | (0x0001 << bit));
            }
            else
            {
                reg = (UInt16)(reg & ~(0x0001 << bit));
            }
        }

        public static bool getBit16(ref UInt16 reg, int bit)
        {
            return 0 != (reg >> bit & 0x0001);
        }

        /// <summary>
        /// 将字节类型的数据转化为十六进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteToHexString(byte data)
        {
            string outString = "";

            if (data < 16)
            {
                outString += "0";
            }
            outString += data.ToString("X");

            return outString;
        }

        /// <summary>
        /// 将字节类型的数据转化为十六进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ByteToHexString(byte[] data, int pos, int length)
        {
            string outString = "";

            for (int i = pos; i < pos + length; i++)
            {
                outString += ByteToHexString(data[i]);
            }

            return outString;
        }
        
        /// <summary>
        /// 将字符串转换为字节类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ","");
            byte[] buffer=new byte[s.Length/2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i,2),16);
            }
            return buffer;
        }


        /// <summary>
        /// 删除指定日期的日志
        /// </summary>
        /// <param name="date"></param>
        public static void DeleteLog(DateTime date)
        {
            string LogFileName = LogPath + "/" + date.ToString("yyyyMMdd") + ".log";

            try
            {
                if (File.Exists(LogFileName))
                {
                    File.Delete(LogFileName);
                }
            }
            catch
            {
            }
        }

        const string LogPath = "./RfidLog/DebugLog";
        const double LogSaveDura_D = 5; 
        /// <summary>
        /// 添加调试记录
        /// </summary>
        /// <param name="str"></param>
        public static void AddDebugLog_L0(string str)
        {
            DeleteLog(System.DateTime.Now.AddDays(0 - LogSaveDura_D));

#if DEBUG_L0_ENABLE
            string LogFileName = LogPath + "/" + System.DateTime.Now.ToString("yyyyMMdd") + ".log";
            string log = System.DateTime.Now.ToString() + " --> " + str + "\r\n";

            lock (LogPath)
            {
                try
                {
                    if (!Directory.Exists(LogPath))
                    {
                        Directory.CreateDirectory(LogPath);
                    }
                    FileStream fs = new FileStream(LogFileName, FileMode.Append);
                    byte[] data = new UTF8Encoding().GetBytes(log);
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch { }
            }
#endif
        }

        /// <summary>
        /// 添加调试记录
        /// </summary>
        /// <param name="str"></param>
        public static void AddDebugLog_L1(string str)
        {
#if DEBUG_L1_ENABLE
            AddDebugLog_L0(str);
#endif
        }

        /// <summary>
        /// 添加调试记录
        /// </summary>
        /// <param name="str"></param>
        public static void AddDebugLog_L2(string str)
        {
#if DEBUG_L2_ENABLE
            AddDebugLog_L1(str);
#endif
        }

        /// <summary>
        /// 将4字节的数据转化为string形式的IP
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string ByteToIpString(byte[] data, int offset)
        {
            string Ip = "";

            if (data.Length > offset + 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Ip += data[offset + i].ToString();
                    if (i < 3)
                    {
                        Ip += ".";
                    }
                }
            }

            return Ip;
        }

        /// <summary>
        /// 将string形式的IP转化为对于的字节
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static byte[] IpStringToByte(string ip)
        {
            try
            {
                System.Net.IPAddress IpAddr = System.Net.IPAddress.Parse(ip);

                return IpAddr.GetAddressBytes();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 比较两个数组指定的位置数据是否相同
        /// </summary>
        /// <param name="data1"></param>
        /// <param name="offset1"></param>
        /// <param name="data2"></param>
        /// <param name="offset2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static bool ByteCmp(byte[] data1, int offset1, byte[] data2, int offset2, int len)
        {
            try
            {
                for (int i = 0; i < len; i++)
                {
                    if (data1[i + offset1] != data2[i + offset2])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
