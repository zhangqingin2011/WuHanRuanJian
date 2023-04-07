﻿using System;
using System.Collections.Generic;
using System.Text;

using Sygole.HFReader;
using System.Threading;

namespace JanusTagInit
{
    public class RfidReader
    {
        HFReader reader = new HFReader();
        object communicationLock = new object();
        object gpiLock = new object();
        int gpi1TriggerCnt = 0;
        int gpi2TriggerCnt = 0;
        public int RFIDDataDataTableDataMax = 500;
        public System.Data.DataTable RFIDReadDataDataTable = new System.Data.DataTable();
        public Object RFIDReadDataDataTable_Look = new object();
        private String RFIDDataDataTable_FileName;
        public string EQUIP_CODE = null;
        public bool writerfidflag = false;
        public bool readrfidflag = true;
        int readerID = 0;
        public string addr;
        public int args;
        static public int TAG_DATA_LENGTH = 17 * 4;
        byte[] nowTagUid = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; //当前标签UID
        byte[] lastTagUid = new byte[8] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; //最后一次初始化成功的标签UID

        public RfidReader(string addr, int args)
        {
            this.addr = addr;
            this.args = args;
            this.readerID = 0;
            String[] Pathstr = SCADA.MainForm.RFIDDataFilePath.Split('\\');
            String[] Filenamestr = addr.Split('.');
            RFIDDataDataTable_FileName = SCADA.MainForm.RFIDDataFilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            RFIDDataDataTable_FileName = RFIDDataDataTable_FileName.Substring(0, RFIDDataDataTable_FileName.Length - 1);
            if (!System.IO.Directory.Exists(RFIDDataDataTable_FileName))
            {
                System.IO.Directory.CreateDirectory(RFIDDataDataTable_FileName);
            }
            RFIDDataDataTable_FileName += "\\" + Pathstr[Pathstr.Length - 1] + Filenamestr[Filenamestr.Length - 1];

            RFIDDataDataTable_XMLFile_load_liaocang(RFIDDataDataTable_FileName);
        }

        public void RFIDDataDataTable_XMLFile_load_liaocang(String FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(FilePath);
                if (fileInfo.Length > 0)
                {
                    RFIDReadDataDataTable = SCADA.RFIDDATAT.DBReadFromXml(FilePath);
                }
            }
            if (RFIDReadDataDataTable.Columns.Count == 0)
            {
                for (int ii = 0; ii < SCADA.RfidForm.RFIDReadDataStruct.Length; ii++)
                {
                    RFIDReadDataDataTable.Columns.Add(SCADA.RfidForm.RFIDReadDataStruct[ii], typeof(string));
                }
            }
        }

        public void AppExit_liaocang()
        {
            SCADA.RFIDDATAT.DBWriteToXml(RFIDReadDataDataTable, RFIDDataDataTable_FileName);
        }

        public int id()
        {
            return readerID;
        }

        public bool connect()
        {
            bool stat = reader.Connect(addr, args);
            for (int cnt = 0; cnt < 3; cnt++)
            {
                if (this.getUserConfig(ref this.readerID) == true) return true;
                Thread.Sleep(600);
            }
            return false;
        }

        public bool isConnect()
        {
            return this.getUserConfig(ref this.readerID);
        }

        public bool write(int value)
        {
//              this.janusClearAll();
            if (this.Inventory_TsRlRt((byte)this.id(), ref this.nowTagUid) == Status_enum.SUCCESS)
            {

                #region 确定工作状态

                bool equl = true;
                for (int i = 0; i < nowTagUid.Length; i++)
                {
                    equl &= (nowTagUid[i] == lastTagUid[i]);
                }
                if (equl == true && writerfidflag == false)
                {
                    return true;
                }

                #endregion

                Dictionary<string, string> dic = null;

                try
                {
                    dic = this.GpiCallBack_Janus();
                }
                catch (Exception ex)
                {
                    dic = null;
                }

                if (dic == null)
                {
                    return false;
                }

                Array.Copy(nowTagUid, lastTagUid, nowTagUid.Length);
                writerfidflag = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool getUserConfig(ref int rid)
        {
            UserCfg cfg = new UserCfg();
            Status_enum stat;
            lock (this.communicationLock)
            {
                stat = reader.GetUserCfg((byte)0x00, ref cfg);
            }
            if (stat == Status_enum.SUCCESS)
            {
                rid = (int)cfg.ReaderID;
                return true;
            }
            else
            {
                return false;
            }

        }

        #region Janus标签初始化

        public Dictionary<string, string> janusReadTag()
        {
            Dictionary<string, string> dic;
            dic = new Dictionary<string, string>();
            dic.Add("uid", "");
            dic.Add("productCode", "");
            dic.Add("opCode", "");
            dic.Add("workCenterCode", "");
            dic.Add("workCellCode", "");
            dic.Add("productID1", "");
            dic.Add("workStatus1", "");
            dic.Add("qualityStatus1", "");
            dic.Add("productID2", "");
            dic.Add("workStatus2", "");
            dic.Add("qualityStatus2", "");

            #region 读UID

            byte[] uid = this.tUid(this.readerID);
            if (uid != null)
            {
                dic["uid"] = HexString.byteToString(uid);
            }
            else
            {
                return null;
            }

            #endregion

            #region 读标签

            byte[] data = this.tRead( this.readerID, 0, 20*4);
            if (data == null)
            {
                return null;
            }

            dic["productCode"] = "";

            #region 读产品编码

            byte[] productID1 = grep(data, 0, 12);
            if (productID1[0] == 0) dic["productID1"] = "";
            else dic["productID1"] = HexASCII.byteToASCII(productID1);

            byte[] productID2 = grep(data, 12, 24);
            if (productID2[0] == 0) dic["productID2"] = "";
            else dic["productID2"] = HexASCII.byteToASCII(productID2);

            #endregion

            byte[] workCenterCode = grep(data, 24, 28);
            if (workCenterCode[0] == 0) dic["workCenterCode"] = "";
            else dic["workCenterCode"] = HexASCII.byteToASCII(workCenterCode);

            #region 工序字符串转byte[] & 读产品加工完成状态

            string OpCodeString = "";
            byte[] readOpCode = grep(data, 28, 30);

            string workStatusString = "";
            byte[] workStatus = grep(data, 30, 32);

            if (readOpCode[0] == 0 && readOpCode[1] == 0 && workStatus[0] == 0 && workStatus[1] == 0)
            {
                dic["opCode"] = "";
                dic["workStatus1"] = ""; ;
                dic["workStatus2"] = ""; ;
            }
            else
            {
                for (int i = 0; i < readOpCode.Length * 8; i++)
                {
                    if (isSetBit(readOpCode, i) == true)
                    {
                        if (OpCodeString.Length > 0) OpCodeString += "-";
                        if (i + 1 < 10) OpCodeString += "J0" + (i + 1).ToString();
                        else OpCodeString += "J" + (i + 1).ToString();

                        //工序完成情况
                        if (isSetBit(workStatus, i) == true)
                        {
                            if (workStatusString.Length > 0) workStatusString += "-";
                            workStatusString += "1";
                        }
                        else
                        {
                            if (workStatusString.Length > 0) workStatusString += "-";
                            workStatusString += "0";
                        }
                    }
                }
                dic["opCode"] = OpCodeString;
                dic["workStatus1"] = workStatusString;
                dic["workStatus2"] = workStatusString;
            }

            #endregion

            dic["workCellCode"] = "";
            dic["qualityStatus1"] = "";
            dic["qualityStatus2"] = "";

            #endregion

            #region 设置数据

            //dic["uid"]=;
            dic["productCode"]="";//不使用
            dic["opCode"]="001-002";//最多4到工序 "01-02-03-04"

            dic["workCellCode"] = "";//不使用
            dic["productID1"] = DateTime.Now.ToFileTime().ToString().Substring(2, 12);//十二位ASCII码
            Thread.Sleep(10);//增加时间，使得productID2的序列号与一不同（此程序可用10年不重复）
            dic["workStatus1"]="0-0";//和opCode的工序数量对应但数据为零
            dic["qualityStatus1"] = "";//不使用
            dic["productID2"] = DateTime.Now.ToFileTime().ToString().Substring(2, 12); Thread.Sleep(10);
            dic["workStatus2"] = "0-0";//和opCode的工序数量对应但数据为零
            dic["qualityStatus2"] = "";//不使用

            if (Collector.CollectHNCPLC.productcode == 2)
            {
                dic["workCenterCode"] = "0001";//写入时只写了后四位
            }
            else if (Collector.CollectHNCPLC.productcode == 4)
            {
                dic["workCenterCode"] = "0002";//写入时只写了后四位
            }
            else
            {
                dic["workCenterCode"] = "";//写入时只写了后四位
            }

            #endregion

            return dic;
        }

        public bool jannusWriteTag(Dictionary<string, string> result)
        {

            #region 格式化即将写入标签的数据
            byte[] tagData = new byte[TAG_DATA_LENGTH];
            for (int i = 0; i < tagData.Length; i++) tagData[i] = (byte)0x00;

            //写入产品编号
            Array.Copy(HexASCII.ASCIIToByte(result["productID1"]), 0, tagData, 0, 12);
            if (result["productID2"] == null || result["productID2"] == "")
            {
                Array.Copy( HexASCII.ASCIIToByte("000000-00000"), 0, tagData, 12, 12);
            }
            else
            {
                Array.Copy(HexASCII.ASCIIToByte(result["productID2"]), 0, tagData, 12, 12);
            }

            //写入线体编号
            Array.Copy(HexASCII.ASCIIToByte(result["workCenterCode"]), (result["workCenterCode"].Length - 4), tagData, 24, 4);

            //写入产品工序与工序完成情况
            byte[] step = new byte[2];
            byte[] completeStep = new byte[2];
            for (int i = 0; i < step.Length; i++)
            {
                step[i] = (byte)0;
                completeStep[i] = (byte)0;
            }
            string[] stepStr = result["opCode"].Split('-');
            int stepCnt = 0;
            foreach (string s in stepStr)
            {
                if (s.Length < 3) return false;
                int loc = (s[1] - '0') * 10 + (s[2] - '0') - 1;
                if (setBit(step, loc) == false)
                {
                    return false;
                }
                if (result["workStatus1"].Length > 0 && result["workStatus1"].Split('-')[stepCnt] == "1")
                {
                    if (setBit(completeStep, loc) == false)
                    {
                        return false;
                    }
                }
                stepCnt++;
            }
            Array.Copy(step, 0, tagData, 28, 2);
            Array.Copy(completeStep, 0, tagData, 30, 2);
            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();

//             SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_RFID;
//             SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//             SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//             SendParm.Keywords = "料盘初始化写入";
//             SendParm.EventData = "tagData=" + HexString.byteToString(tagData);
//             SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

            /*result["productCode"];
            result["workCellCode"];*/

            #endregion

            #region 写标签
            if ( this.tWrite(this.readerID, 0, tagData.Length, tagData) == false)
            {
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_RFID;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = "料盘初始化写入失败";
                SendParm.EventData = "";
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

                return false;
            }

            #endregion

            return true;
        }


        public byte[] janusReadAll()
        {
            byte[] data = this.tRead(this.readerID, 0, TAG_DATA_LENGTH);
            if (data == null)
            {
                return null;
            }
            return data;
/*            return HexString.byteToString(data);*/
        }

        public bool janusClearAll()
        {
            byte[] tagData = new byte[TAG_DATA_LENGTH];
            for (int i = 0; i < tagData.Length; i++) tagData[i] = (byte)0x00;
            if (this.tWrite(this.readerID, 0, tagData.Length, tagData) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Dictionary<string, string> GpiCallBack_Janus()
        {
            Dictionary<string, string> dic = null;

            dic = janusReadTag();
            if (dic == null)
            {
                throw new Exception("读写器未能读出标签数据");
            }
            if (jannusWriteTag(dic) == false)
            {
                throw new Exception("读写器写标签错误");
            }

            return dic;
        }

        /// <summary>
        /// 截取部分字节数组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        byte[] grep(byte[] source, int start, int end)
        {
            byte[] res;
            if (start < 0 && start >= end && end > source.Length)
            {
                res = new byte[0];
            }
            else
            {
                res = new byte[end - start];
                Array.Copy(source, start, res, 0, res.Length);
            }
            return res;
        }

        /// <summary>
        /// 向destination中填入source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        int fill(byte[] source, byte[] destination, int start, int end)
        {
            if (start < 0 && start >= end &&
                source.Length > end - start && destination.Length > end)
            {
                return 0;
            }
            Array.Copy(source, 0, destination, start, end - start);
            return end - start;
        }

        bool isSetBit(byte[] barr, int location)
        {
            int byteLocation = location / 8;
            int bitLocation = location % 8;
            if (byteLocation >= 0 && bitLocation >= 0 &&
                barr == null && barr.Length < byteLocation + 1)
            {
                return false;
            }
            else
            {
                return (barr[byteLocation] & (byte)(0x01 << bitLocation)) != 0x00;
            }
        }

        bool setBit(byte[] barr, int location)
        {
            int byteLocation = location / 8;
            int bitLocation = location % 8;
            if (byteLocation >= 0 && bitLocation >= 0 &&
                barr == null && barr.Length < byteLocation + 1)
            {
                return false;
            }
            else
            {
                barr[byteLocation] |= (byte)(0x01 << bitLocation);
                return true;
            }
        }

        bool resetBit(byte[] barr, int location)
        {
            int byteLocation = location / 8;
            int bitLocation = location % 8;
            if (byteLocation >= 0 && bitLocation >= 0 &&
                barr == null && barr.Length < byteLocation + 1)
            {
                return false;
            }
            else
            {
                barr[byteLocation] |= (byte)(~(0x01 << bitLocation));
                return true;
            }
        }

        #endregion

        #region 可靠加强标签读写

        public Status_enum Inventory_Ts(byte ReaderID, ref byte[] UID)
        {
            if (reader == null) return Status_enum.FAILURE;
            Status_enum stat;
            lock (this.communicationLock)
            {
                stat = reader.Inventory(ReaderID, ref UID);
            }
            return stat;
        }

        public Status_enum Inventory_TsRl(byte ReaderID, ref byte[] UID)
        {
            byte[] UIDB = new byte[UID.Length];
            Status_enum statA, statB;

            //读取两次
            statA = Inventory_Ts(ReaderID, ref UID);
            statB = Inventory_Ts(ReaderID, ref UIDB);

            //两次中任何一次有失败，返回其失败状态
            if (Status_enum.SUCCESS != statA) return statA;
            if (Status_enum.SUCCESS != statB) return statB;

            //两次数据不相等，返回失败
            for (int j = 0; j < UID.Length; j++)
            {
                if (UID[j] != UIDB[j])
                {
                    return Status_enum.FAILURE;
                }
            }

            //返回成功
            return Status_enum.SUCCESS;
        }

        public Status_enum Inventory_TsRlRt(byte ReaderID, ref byte[] UID, int retry = 2)
        {
            Status_enum stat = Status_enum.FAILURE;
            if (retry < 1) retry = 1;
            for (int i = 0; i < retry; i++)
            {
                stat = Inventory_TsRl(ReaderID, ref UID);
                if (stat == Status_enum.SUCCESS)
                {
                    break;
                }
            }
            return stat;
        }

        public Status_enum GetTagInfo_Ts(byte ReaderID, Opcode_enum Opcode, byte[] UID, ref TagInfo info, Antenna_enum ant)
        {
            Status_enum stat;
            lock (this.communicationLock)
            {
                stat = reader.GetTagInfo(ReaderID, Opcode, UID, ref info, ant);
            }
            return stat;
        }

        public Status_enum GetTagInfo_TsRl(byte ReaderID, Opcode_enum Opcode, byte[] UID, ref TagInfo info, Antenna_enum ant)
        {
            TagInfo infoB = new TagInfo();
            Status_enum statA, statB;

            //读取两次
            statA = GetTagInfo_Ts(ReaderID, Opcode, UID, ref info, ant);
            statB = GetTagInfo_Ts(ReaderID, Opcode, UID, ref infoB, ant);

            //两次中任何一次有失败，返回其失败状态
            if (Status_enum.SUCCESS != statA) return statA;
            if (Status_enum.SUCCESS != statB) return statB;

            //两次数据不相等，返回失败
            bool equl = true;
            equl &= Array.Equals(info.UID, infoB.UID);
            equl &= (info.BlockCnt == infoB.BlockCnt);
            equl &= (info.BlockSize == infoB.BlockSize);

            if (equl == false)
            {
                return Status_enum.FAILURE;
            }

            //返回成功
            return Status_enum.SUCCESS;
        }

        public Status_enum GetTagInfo_TsRlRt(byte ReaderID, Opcode_enum Opcode, byte[] UID, ref TagInfo info, Antenna_enum ant, int retry = 20)
        {
            Status_enum stat = Status_enum.FAILURE;
            if (retry < 1) retry = 1;
            for (int i = 0; i < retry; i++)
            {
                stat = GetTagInfo_Ts(ReaderID, Opcode, UID, ref info, ant);
                if (stat == Status_enum.SUCCESS)
                {
                    break;
                }
            }
            return stat;
        }

        public Status_enum ReadMBlock_Ts(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] datas, ref byte len)
        {
            Status_enum stat;
            lock (this.communicationLock)
            {
                stat = reader.ReadMBlock(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref datas, ref len);
            }
            return stat;
        }

        public Status_enum ReadMBlock_TsRl(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] datas, ref byte len)
        {
            byte[] dataB = new byte[datas.Length];
            byte lenB = len;
            Status_enum statA, statB;

            //读取两次
            statA = ReadMBlock_Ts(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref datas, ref len);
            statB = ReadMBlock_Ts(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref dataB, ref lenB);

            //两次中任何一次有失败，返回其失败状态
            if (Status_enum.SUCCESS != statA) return statA;
            if (Status_enum.SUCCESS != statB) return statB;

            //两次长度不相等，返回失败
            if (len != lenB)
            {
                return Status_enum.FAILURE;
            }

            //两次数据不相等，返回失败
            for (int j = 0; j < len; j++)
            {
                if (datas[j] != dataB[j])
                {
                    return Status_enum.FAILURE;
                }
            }

            //返回成功
            return Status_enum.SUCCESS;
        }

        public Status_enum ReadMBlock_TsRlRt(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, ref byte[] datas, ref byte len, int retry = 20)
        {
            byte llen = len;
            Status_enum stat = Status_enum.FAILURE;
            if (retry < 1) retry = 1;
            for (int i = 0; i < retry; i++)
            {
                len = llen;
                stat = ReadMBlock_TsRl(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref datas, ref len);
                if (stat == Status_enum.SUCCESS)
                {
                    break;
                }
            }
            return stat;
        }

        public Status_enum ReadMBlock_TsRlRtEx(byte ReaderID, Opcode_enum Opcode, byte[] UID, int StartBlock, int BlockCnt, ref byte[] datas, ref int len, int retry = 20)
        {
            int datasLen = 0;
            byte[] lData = new byte[72];
            byte lLen = (byte)lData.Length;
            Status_enum stat = Status_enum.SUCCESS;
            int lcnt = 0;
            byte lStartBlock;
            byte lBlockCnt;

            if (StartBlock < 0 || BlockCnt <= 0)
            {
                return Status_enum.FAILURE;
            }

            //以8块为单位读出块数据
            for (lcnt = 0; lcnt < BlockCnt / 8 + 1; lcnt++)
            {
                if (lcnt < BlockCnt / 8)
                {
                    lStartBlock = (byte)(lcnt * 8 + StartBlock);
                    lBlockCnt = (byte)8;
                    lLen = (byte)lData.Length;
                }
                else
                {
                    //读出剩余块
                    lStartBlock = (byte)(lcnt * 8 + StartBlock);
                    lBlockCnt = (byte)(BlockCnt % 8);
                    lLen = (byte)lData.Length;
                }

                stat = ReadMBlock_TsRlRt(ReaderID, Opcode, UID, lStartBlock, lBlockCnt, ref lData, ref lLen, retry);
                if (datasLen + lLen > len) return Status_enum.FAILURE;
                Array.Copy(lData, 0, datas, datasLen, lLen);
                datasLen += lLen;

                if (Status_enum.SUCCESS != stat)
                {
                    return stat;
                }
            }

            len = datasLen;
            return Status_enum.SUCCESS;
        }

        public Status_enum WriteMBlock_Ts(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, int BlockSize, byte[] BlockDatas)
        {
            if (reader == null) return Status_enum.FAILURE;
            Status_enum stat;
            lock (this.communicationLock)
            {
                stat = reader.WriteMBlock(ReaderID, Opcode, UID, StartBlock, BlockCnt, BlockSize, BlockDatas);
            }
            return stat;
        }

        public Status_enum WriteMBlock_TsRl(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, int BlockSize, byte[] BlockDatas)
        {
            Status_enum stat;
            byte len = (byte)BlockDatas.Length;
            byte[] datas = new byte[len];

            stat = WriteMBlock_Ts(ReaderID, Opcode, UID, StartBlock, BlockCnt, BlockSize, BlockDatas);
            if (stat != Status_enum.SUCCESS) return stat;

            stat = ReadMBlock_TsRl(ReaderID, Opcode, UID, StartBlock, BlockCnt, ref datas, ref len);
            if (stat != Status_enum.SUCCESS) return stat;

            for (int i = 0; i < len; i++)
            {
                if (BlockDatas[i] != datas[i])
                {
                    return Status_enum.FAILURE;
                }
            }

            return Status_enum.SUCCESS;
        }

        public Status_enum WriteMBlock_TsRlRt(byte ReaderID, Opcode_enum Opcode, byte[] UID, byte StartBlock, byte BlockCnt, int BlockSize, byte[] BlockDatas, int retry = 20)
        {
            Status_enum stat = Status_enum.FAILURE;
            if (retry < 1) retry = 1;
            for (int i = 0; i < retry; i++)
            {
                stat = WriteMBlock_TsRl(ReaderID, Opcode, UID, StartBlock, BlockCnt, BlockSize, BlockDatas);
                if (stat == Status_enum.SUCCESS)
                {
                    break;
                }
            }
            return stat;
        }

        public Status_enum WriteMBlock_TsRlRtEx(byte ReaderID, Opcode_enum Opcode, byte[] UID, int StartBlock, int BlockCnt, int BlockSize, byte[] BlockDatas, int retry = 20)
        {
            int datasLen = 0;
            byte[] lData = new byte[72];
            byte lLen = (byte)lData.Length;
            Status_enum stat = Status_enum.SUCCESS;
            int lcnt = 0;
            byte lStartBlock;
            byte lBlockCnt;

            if (StartBlock < 0 || BlockCnt <= 0 || (BlockSize != 4 && BlockSize != 8))
            {
                return Status_enum.FAILURE;
            }

            for (lcnt = 0; lcnt < BlockCnt / 8 + 1; lcnt++)
            {
                if (lcnt < BlockCnt / 8)
                {
                    lStartBlock = (byte)(lcnt * 8 + StartBlock);
                    lBlockCnt = (byte)8;
                }
                else
                {
                    lStartBlock = (byte)(lcnt * 8 + StartBlock);
                    lBlockCnt = (byte)(BlockCnt % 8);
                }

                Array.Copy(BlockDatas, datasLen, lData, 0, lBlockCnt * BlockSize);
                stat = WriteMBlock_TsRlRt(ReaderID, Opcode, UID, lStartBlock, lBlockCnt, BlockSize, lData, retry);
                datasLen += (byte)(lBlockCnt * BlockSize);

                if (Status_enum.SUCCESS != stat)
                {
                    return stat;
                }
            }

            return Status_enum.SUCCESS;
        }

        #endregion

        public void UpDataReadData2Table_liaocang(string[] rfiddata)
        {
            lock (RFIDReadDataDataTable_Look)
            {
                if (RFIDReadDataDataTable.Rows.Count >= RFIDDataDataTableDataMax)
                {
                    RFIDReadDataDataTable.Rows.RemoveAt(RFIDDataDataTableDataMax - 1);
                }
                System.Data.DataRow r;
                r = RFIDReadDataDataTable.NewRow();
                RFIDReadDataDataTable.Rows.Add(r);
                for (int ii = RFIDReadDataDataTable.Rows.Count - 1; ii > 0; ii--)
                {
                    for (int jj = 1; jj < RFIDReadDataDataTable.Columns.Count; jj++)
                    {
                        RFIDReadDataDataTable.Rows[ii][jj] = RFIDReadDataDataTable.Rows[ii - 1][jj];
                    }
                    RFIDReadDataDataTable.Rows[ii][0] = ii;
                }

                RFIDReadDataDataTable.Rows[0][0] = 0;
                RFIDReadDataDataTable.Rows[0][1] = DateTime.Now.ToString();
                for (int jj = 0; jj < rfiddata.Length; jj++)
                {
                    RFIDReadDataDataTable.Rows[0][jj + 2] = rfiddata[jj];
                }
            }
        }

        #region 可靠加强虚拟化操作

        public byte[] tUid(int ReaderID)
        {
            byte[] uid = new byte[8];
            if (Inventory_TsRlRt((byte)ReaderID, ref uid, 2) != Status_enum.SUCCESS)
            {
                return null;
            }
            else
            {
                return uid;
            }
        }

        public bool tInfo(int ReaderID, ref int BlockCnt, ref int BlockSize)
        {
            TagInfo info = new TagInfo();
            if (GetTagInfo_TsRlRt((byte)ReaderID, Opcode_enum.NON_ADDRESS_MODE, null, ref info, Antenna_enum.ANT_1, 2) != Status_enum.SUCCESS)
            {
                return false;
            }
            //Array.Copy(info.UID, UID, UID.Length);
            BlockCnt = info.BlockCnt + 1;
            BlockSize = info.BlockSize + 1;
            return true;
        }

        public byte[] tRead(int ReaderID, int startByte, int endByte)
        {
            byte[] datas = new byte[endByte - startByte];

            int lBlockCnt = 0;
            int lBlockSize = 0;

            if (tInfo(ReaderID, ref lBlockCnt, ref lBlockSize) != true) return null;
            if (lBlockSize == 0 || lBlockSize == 0) return null;

            int lStartBlock = startByte / lBlockSize;
            int lEndBlock = (endByte - 1) / lBlockSize;

            int lStartByte = startByte % lBlockSize;
            int lEndByte = (endByte - 1) % lBlockSize + (lEndBlock - lStartBlock) + 1;

            byte[] TmpDatas = new byte[((lEndBlock - lStartBlock) + 1) * lBlockSize];
            int TmpDatasLen = TmpDatas.Length;

            if (lStartBlock <= lEndBlock)
            {
                if (Status_enum.SUCCESS == ReadMBlock_TsRlRtEx((byte)ReaderID, Opcode_enum.NON_ADDRESS_MODE, null, lStartBlock, (lEndBlock - lStartBlock) + 1, ref TmpDatas, ref TmpDatasLen, 2))
                {
                    Array.Copy(TmpDatas, lStartByte, datas, 0, datas.Length);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return datas;
        }

        //注意写失败可能会造成标签数据破坏
        public bool tWrite(int ReaderID, int startByte, int endByte, byte[] datas)
        {
            int lBlockCnt = 0;
            int lBlockSize = 0;

            if (datas.Length < endByte - startByte) return false;
            if (tInfo(ReaderID, ref lBlockCnt, ref lBlockSize) != true) return false;
            if (lBlockSize == 0 || lBlockSize == 0) return false;

            int lStartBlock = startByte / lBlockSize;
            int lEndBlock = (endByte - 1) / lBlockSize + 1;

            int lStartByte = startByte % lBlockSize;
            int lEndByte = (endByte - 1) % lBlockSize + (lEndBlock - lStartBlock);

            byte[] TmpBlock = new byte[lBlockSize];
            int TmpBlockLen = lBlockSize;
            byte[] TmpDatas = new byte[(lEndBlock - lStartBlock) * lBlockSize];
            int TmpDatasLen = TmpDatas.Length;

            if ((lEndBlock - lStartBlock) < 1)
            {
                return false;
            }

            if ((lEndBlock - lStartBlock) >= 1)
            {
                if (ReadMBlock_TsRlRtEx((byte)ReaderID, Opcode_enum.NON_ADDRESS_MODE, null, lStartBlock, 1, ref TmpBlock, ref TmpBlockLen, 2) != Status_enum.SUCCESS) return false;
                Array.Copy(TmpBlock, 0, TmpDatas, 0, lBlockSize);
            }

            if ((lEndBlock - lStartBlock) >= 2)
            {
                if (ReadMBlock_TsRlRtEx((byte)ReaderID, Opcode_enum.NON_ADDRESS_MODE, null, lEndBlock - 1, 1, ref TmpBlock, ref TmpBlockLen, 2) != Status_enum.SUCCESS) return false;
                Array.Copy(TmpBlock, 0, TmpDatas, TmpDatas.Length - lBlockSize, lBlockSize);
            }

            Array.Copy(datas, 0, TmpDatas, lStartByte, endByte - startByte);
            if (WriteMBlock_TsRlRtEx((byte)ReaderID, Opcode_enum.NON_ADDRESS_MODE, null, lStartBlock, lEndBlock - lStartBlock, lBlockSize, TmpDatas, 2) != Status_enum.SUCCESS)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }

    public class HexString
    {
        /// <summary>
        /// 十六进制字符样本，其序号就是其所代表的值
        /// </summary>
        private static string hex_std = "0123456789ABCDEF";
        public byte[] hex;

        public HexString()
        {
            hex = null;
        }

        public HexString(string s)
        {
            if (s == null)
            {
                hex = null;
                return;
            }

            string hexStr = adjust(s);
            if (hexStr != null)
            {
                hex = stringToByte(hexStr);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(string s, int hexLen)
        {
            if (s == null || hexLen == 0 || hexLen * 2 > s.Length)
            {
                hex = null;
                return;
            }

            string hexStr = adjust(s);
            if (hexStr != null)
            {
                hex = stringToByte(hexStr, hexLen);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(byte[] barr)
        {
            if (barr != null && barr.Length > 0)
            {
                hex = new byte[barr.Length];
                Array.Copy(barr, hex, barr.Length);
            }
            else
            {
                hex = null;
            }
        }

        public HexString(byte[] barr, int len)
        {
            if (barr != null && barr.Length > 0 && barr.Length >= len)
            {
                hex = new byte[len];
                Array.Copy(barr, hex, len);
            }
            else
            {
                hex = null;
            }
        }

        public int len()
        {
            if (hex == null)
            {
                return 0;
            }
            else
            {
                return hex.Length;
            }
        }

        public static string adjust(string hexstr)
        {
            hexstr = hexstr.ToUpper();
            string s = "";
            //去除字符串中的非大写十六进制字符
            for (int i = 0; i < hexstr.Length; i++)
            {
                if (hex_std.IndexOf(hexstr[i]) >= 0)
                {
                    s += hexstr[i];
                }
            }
            //检测是否为偶数个字符
            if (s.Length == 0 || s.Length % 2 != 0)
            {
                return null;
            }
            else
            {
                return s;
            }
        }

        /// <summary>
        /// 截取二进制数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns>如果超出长度，返回null</returns>
        public byte[] getHex(int start, int size)
        {
            if (this.hex.Length < start + size)
            {
                return null;
            }

            byte[] barr = new byte[size];
            for (int i = 0; i < size; i++)
            {
                barr[i] = this.hex[start + i];
            }
            return barr;
        }

        public static byte[] stringToByte(string hexstr, int hexLen = 0)
        {
            byte[] barr;
            string s = adjust(hexstr);
            if (s == null || hexLen * 2 > hexstr.Length)
            {
                return null;
            }

            if (hexLen == 0)
            {
                hexLen = s.Length / 2;

            }

            barr = new byte[hexLen];
            for (int i = 0; i < barr.Length; i++)
            {
                barr[i] = (byte)(hex_std.IndexOf(s[i * 2]) * 16 + hex_std.IndexOf(s[i * 2 + 1]));
            }
            return barr;
        }

        public static string byteToString(byte[] barr, int hexLen = 0)
        {
            string s = "";
            if (barr == null || hexLen > barr.Length)
            {
                return s;
            }
            if (hexLen == 0)
            {
                hexLen = barr.Length;
            }
            for (int i = 0; i < hexLen; i++)
            {
                s += hex_std[((barr[i] >> 4) & 0x0f)];
                s += hex_std[((barr[i]) & 0x0f)];
            }
            return s;
        }


        public override string ToString()
        {
            return byteToString(hex);
        }


        public string ToString(int groupLen, string seperator = " ")
        {
            string s = this.ToString();
            string ret = "";
            int arrLen = this.len();

            //如果分组长度不大于0，则设置为每个byte都加入空格
            if (groupLen <= 0)
            {
                groupLen = 1;
            }

            for (int i = 0; i < arrLen; i++)
            {
                ret += s[2 * i];
                ret += s[2 * i + 1];
                if ((i + 1) % groupLen == 0 && i != arrLen - 1)
                {
                    ret += seperator;
                }
            }
            return ret;
        }
    }

    public class HexASCII
    {
        public byte[] hex;

        public HexASCII()
        {
            hex = null;
        }

        public HexASCII(string s)
        {
            if (s == null)
            {
                hex = null;
                return;
            }

            hex = ASCIIToByte(s);

        }

        public HexASCII(string s, int hexLen)
        {
            if (s == null || hexLen == 0 || hexLen * 2 > s.Length)
            {
                hex = null;
                return;
            }

            hex = ASCIIToByte(s, hexLen);
        }

        public HexASCII(byte[] barr)
        {
            if (barr != null && barr.Length > 0)
            {
                hex = new byte[barr.Length];
                Array.Copy(barr, hex, barr.Length);
            }
            else
            {
                hex = null;
            }
        }

        public HexASCII(byte[] barr, int len)
        {
            if (barr != null && barr.Length > 0 && barr.Length >= len)
            {
                hex = new byte[len];
                Array.Copy(barr, hex, len);
            }
            else
            {
                hex = null;
            }
        }

        public int len()
        {
            if (hex == null)
            {
                return 0;
            }
            else
            {
                return hex.Length;
            }
        }

        public static string byteToASCII(byte[] barr, int hexLen = 0)
        {
            string s = "";
            if (barr == null || hexLen > barr.Length)
            {
                return s;
            }
            if (hexLen == 0)
            {
                hexLen = barr.Length;
            }
            for (int i = 0; i < hexLen; i++)
            {
                s += (char)barr[i];
            }
            return s;
        }

        public static byte[] ASCIIToByte(string asciistr, int hexLen = 0)
        {
            byte[] barr;
            string s = asciistr;
            if (s == null || hexLen > asciistr.Length)
            {
                return null;
            }

            if (hexLen == 0)
            {
                hexLen = s.Length;
            }

            barr = new byte[hexLen];
            for (int i = 0; i < barr.Length; i++)
            {
                barr[i] = (byte)(asciistr[i]);
            }
            return barr;
        }

        public override string ToString()
        {
            return byteToASCII(hex);
        }
    }

}
