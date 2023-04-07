using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sygole.HFReader;
using System.Threading;


namespace SCADA.NewApp
{
    public partial class SygoleRFID
    {
        private byte ReaderID = 0x00;//读写器ID
        private Opcode_enum opcode = Opcode_enum.ADDRESS_MODE;
        private byte blockStart = 0, blockCnt = 2, blockSize = 4; //多块读写  blocksize只能4和8
        private HFReader SGreader = new HFReader();
        public string IP;
        public int Port;

        public ConnectStatusEnum ConnectStatus
        {
            get { return SGreader.ConnectStatus; }
        }

        public bool Connect(string ip, int port)
        {
            IP = ip;
            Port = port;
            return SGreader.Connect(ip, port);
        }

        public void DisConnect()
        {
            SGreader.DisConnect();
        }

        public bool ReConnect()
        {
            bool flag = false;
            flag = SGreader.Connect(IP, Port);
            if (flag)
            {
                return true;
            }
            else
            {
                Thread.Sleep(50);
                flag = SGreader.Connect(IP, Port);
                if (flag)
                    return true;
                else
                    return false;
            }
        }

        public string readRFIDUID()
        {
            byte[] UID = new byte[9];
            string readuid = null;
            bool flag = ReConnect();
            if (!flag)
            {
                return readuid;
            }

            int pos = 0;
            int length = UID.Length - 1;
            if (SGreader.Inventory(ReaderID, ref UID) == Status_enum.SUCCESS)
            {
                readuid = ByteToHexString(UID, pos, length);
                return readuid;
            }
            else
            {
                readuid = null;
                return readuid;
            }
        }

        public string readRFIDData()
        {
            byte[] UID = new byte[9];
            Antenna_enum ant = Antenna_enum.ANT_1;
            Opcode_enum Opcode = new Opcode_enum();
            byte[] datas = new byte[9];
            byte len = 0;
            int pos = 0;
            int length = datas.Length - 1;
            string rfiddata = null;
            if (Status_enum.SUCCESS == SGreader.ReadMBlock(ReaderID, Opcode, UID, blockStart, blockCnt, ref datas, ref len, ant))
            {
                rfiddata = ByteToHexString(datas, pos, length);
            }
            return rfiddata;
        }

        public bool writeRFIDData(string rfiddata)
        {
            byte[] UID = new byte[9];
            Opcode_enum Opcode = new Opcode_enum();
            byte BlockSize = 4;//块大小    
            byte[] BlockDatas = HexStringToByteArray(rfiddata);

            if (Status_enum.SUCCESS == SGreader.WriteMBlock(ReaderID, Opcode, UID, blockStart, blockCnt, BlockSize, BlockDatas))
            {
                return true;
            }
            else
            {
                return false;
            }
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

        //将字节类型的数据转化为十六进制字符串
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
        /// 将字符串转换为字节类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }
            return buffer;
        }
    }
}
