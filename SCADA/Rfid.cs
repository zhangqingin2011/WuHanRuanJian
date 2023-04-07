using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using Sygole.HFReader;

namespace SCADA
{
    #region    ��Ķ���
    public class RFID : ObservableObject
    {
        private HFReader reader = null;
        public delegate void HNC8RFIDEventHandler<HNC8PLCRFIDEvent>(object sender, PLC.HNC8PLCRFIDEvent Args);
        public static event HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent> AutoSendHandler = null;//�����������Ϣ
        public event HNC8RFIDEventHandler<PLC.HNC8PLCRFIDEvent> StateChangeHandler = null;//����PLC������״̬
        public int RFIDDataDataTableDataMax = 500;
        public System.Data.DataTable RFIDReadDataDataTable = null;
        public Object RFIDReadDataDataTable_Look = null;
        public String[] RFIDDataStr = null;
        public RFID(string linkAddress, string linkPort, ref String[] RFIDDataStr)
        {
            reader = new HFReader();
            RFIDReadDataDataTable = new System.Data.DataTable();
            RFIDReadDataDataTable_Look = new Object();
            this.linkAddress = linkAddress;
            this.linkPort = linkPort;
            this.RFIDDataStr = RFIDDataStr;
            String[] Pathstr = SCADA.MainForm.RFIDDataFilePath.Split('\\');
            String[] Filenamestr = linkAddress.Split('.');
            RFIDDataDataTable_FileName = SCADA.MainForm.RFIDDataFilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            RFIDDataDataTable_FileName = RFIDDataDataTable_FileName.Substring(0, RFIDDataDataTable_FileName.Length - 1);
            if (!System.IO.Directory.Exists(RFIDDataDataTable_FileName))
            {
                System.IO.Directory.CreateDirectory(RFIDDataDataTable_FileName);
            }
            RFIDDataDataTable_FileName += "\\" + Pathstr[Pathstr.Length - 1] + Filenamestr[Filenamestr.Length - 1];

            RFIDDataDataTable_XMLFile_load(RFIDDataDataTable_FileName);
            this.initLink();
        }

        private String RFIDDataDataTable_FileName;
        public void AppExit()
        {
            SCADA.RFIDDATAT.DBWriteToXml(RFIDReadDataDataTable, RFIDDataDataTable_FileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void RFIDDataDataTable_XMLFile_load(String FilePath)
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

        #region RFID����
        /// <summary>
        /// ���ӵ�ַ�ַ���
        /// </summary>
        private string linkAddress = "192.168.1.214";//IP
        private string linkPort = "10001";//Port
        private int readerID;
        private String _setStatus = "";
        public String setStatus
        {
            get
            {
                return this._setStatus;
            }
            set
            {
                if (this._setStatus != value)
                {
                    this._setStatus = value;
//                     PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
//                     m_SendData.RFIDserial = this.RFIDserial;
//                     m_SendData.EventType = 100;
//                     m_SendData.Event = value;
//                     if (RFID.AutoSendHandler != null)
//                     {
//                         RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
//                     }
                }
            }
        }

        public int RFIDserial;
        /// <summary>
        /// ʵʱ������״̬
        /// </summary>
        public StatusVM<LinkStatusEnum> linkStatus
        {
            get;
            set;
        }

        /// <summary>
        /// ״̬ˢ�¼��
        /// </summary>
        public int linkStatusCheckInterval = 5000;

        /// <summary>
        /// ��������Դ���
        /// </summary>
        public void UpDataReadData2Table()
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
                for (int jj = 0; jj < RFIDDataStr.Length; jj++)
                {
                    RFIDReadDataDataTable.Rows[0][jj + 2] = RFIDDataStr[jj];
                }
            }
//             PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
//             m_SendData.EventType = 200;
//             if (SCADA.RFID.AutoSendHandler != null)
//             {
//                 SCADA.RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
//             }
        }

        #endregion


        #region ��������

        /// <summary>
        /// ����ģʽ��ȫ���ʣ��빦�ʣ�
        /// </summary>
        private PowerModeEnum _powerMode;
        public PowerModeEnum powerMode
        {
            get { return _powerMode; }
            set { setValue(ref _powerMode, value, "powerMode"); }
        }

        /// <summary>
        /// �������ʱ�䣨ֱ���ɹ���ִֻ��һ�Σ�
        /// </summary>
        private CommandModeEnum _commandMode;
        public CommandModeEnum commandMode
        {
            get { return _commandMode; }
            set { setValue(ref _commandMode, value, "commandMode"); }
        }

        /// <summary>
        /// �������ݷ���ͨѶ��ʽ
        /// </summary>
        private ServiceCommunicationModeEnum _serviceCommunicationMode;
        public ServiceCommunicationModeEnum serviceCommunicationMode
        {
            get { return _serviceCommunicationMode; }
            set { setValue(ref _serviceCommunicationMode, value, "serviceCommunicationMode"); }
        }

        /// <summary>
        /// �̼��汾��
        /// </summary>
        private string _framewareVersion;
        public string framewareVersion
        {
            get { return _framewareVersion; }
            set { setValue(ref _framewareVersion, value, "framewareVersion"); }
        }

        #endregion

        #region ���÷���

        public void getUserConfig()
        {
            UserCfg cfg = new UserCfg();
            if (reader.GetUserCfg((byte)0x00, ref cfg) == Status_enum.SUCCESS)
            {
                this.readerID = (int)cfg.ReaderID;
                reader.SetReaderID(this.readerID);
                this.commandMode = (CommandModeEnum)cfg.AvailableTime;
                this.serviceCommunicationMode = (ServiceCommunicationModeEnum)cfg.CommPort;
                this.powerMode = (PowerModeEnum)cfg.RFChipPower;
            }
        }

        public void setUserConfig()
        {
            UserCfg cfg = new UserCfg();
            Status_enum stat;

            stat = reader.GetUserCfg((byte)0x00, ref cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }

            cfg.ReaderID = (byte)this.readerID;
            cfg.AvailableTime = (AvailableTime_enum)this.commandMode;
            cfg.CommPort = (CommPort_enum)this.serviceCommunicationMode;
            cfg.RFChipPower = (RFChipPower_enum)this.powerMode;

            stat = reader.SetUserCfg((byte)this.readerID, cfg);
            if (stat == Status_enum.SUCCESS)
            {
                return;
            }
        }

        public void getFramewareVersion()
        {
            byte[] ver = new byte[20];
            byte len = (byte)ver.Length;
            for (int i = 0; i < len; i++) { ver[i] = 0x00; }
            Status_enum stat = reader.GetSWVer((byte)this.readerID, ref ver, ref len);
            if (stat != Status_enum.SUCCESS) return;
            string v = "";
            for (int i = 0; i < len; i++)
            {
                v += (char)ver[i];
            }
            System.Text.Encoding.ASCII.GetString(ver).Trim();
            this.framewareVersion = v.ToString();
        }

        public void setDefaultUserConfig()
        {
            UserCfg cfg = new UserCfg();
            Status_enum stat;

            stat = reader.GetUserCfg((byte)0x00, ref cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }

            this.commandMode = CommandModeEnum.JustOnce;
            this.serviceCommunicationMode = ServiceCommunicationModeEnum.RS_232;
            this.powerMode = PowerModeEnum.Full;

            cfg.AvailableTime = (AvailableTime_enum)this.commandMode;
            cfg.CommPort = (CommPort_enum)this.serviceCommunicationMode;
            cfg.RFChipPower = (RFChipPower_enum)this.powerMode;

            stat = reader.SetUserCfg((byte)this.readerID, cfg);
            if (stat != Status_enum.SUCCESS)
            {
                return;
            }
        }

        public void resetMCU()
        {
            reader.SoftReset((byte)this.readerID);
        }

        #endregion

        #region ���ӷ���

        /// <summary>
        /// ��ʼ������
        /// </summary>
        public void initLink()
        {
            if (reader == null) return;
            this.autoLinkStatus = ServiceStatusEnum.Closed;
            this.linkStatus = new StatusVM<LinkStatusEnum>(this, this.getLinkStatus);
            this.linkStatus.now = LinkStatusEnum.Unlink;
            this.linkStatus.StatusChanged += this.linkStatusChangeHandler;
            this.linkStatus.LinkEvent += this.LinkEventHandler;
        }

        private void LinkEventHandler(StatusVM<LinkStatusEnum> sender)
        {
            if (this.autoLinkStatus != ServiceStatusEnum.Opened)
            {
                return;
            }

            //���״̬����������Ҫ����
            //             if (LinkStatusEnum.Linked != sender.now && LinkStatusEnum.Unlinking != sender.now && LinkStatusEnum.Linked == sender.last)
            if (sender.now == LinkStatusEnum.Unlink)
            {
                try
                {
                    if (ClientPingTest(linkAddress))
                    {
                        this.connect();
                    }
                    else
                    {
                        this.setStatus = "�������";
                    }
                }
                catch { ;}
            }

        }
        private Boolean ClientPingTest(String ip)
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send(ip, 1000);
                if (pingReply.Status == System.Net.NetworkInformation.IPStatus.Success)
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

        public void connect()
        {
            if (this.linkAddress == "" || this.linkPort == "")
            {
                //                 this.setStatus("���ӵ�ַ����");
                this.linkStatus.now = LinkStatusEnum.Unlink;
                return;
            }
            if (this.linkStatus.now == LinkStatusEnum.Linked || this.linkStatus.now == LinkStatusEnum.Linking)
            {
                return;
            }
            if (reader.Connect(this.linkAddress, int.Parse(this.linkPort)))
            {
//                 TimeCounter tc = new TimeCounter();
//                 tc.start();
//                 while (tc.now() < 500 && this.linkStatus.now != LinkStatusEnum.Linked)
                {
                    this.getUserConfig();
                    this.linkStatus.update();
                }
            }


            //             if (this.linkStatus.now != LinkStatusEnum.Linked)
            //             {
            //                 this.disconnect();
            //             }
            //this.linkStatus.now = LinkStatusEnum.Linked;
            //this.addCommEvent();  //��������
        }

        public void disconnect()
        {
            this.stopCheck();
            reader.DisConnect();
            this.linkStatus.reset();
            this.linkStatus.now = LinkStatusEnum.Unlink;
        }


        /// <summary>
        /// �����Զ����״̬���Զ�����
        /// </summary>
        public void startCheck()
        {
            if (reader == null) return;
            if (this.linkStatus.isChecking() == false)
            {
                this.autoLinkStatus = ServiceStatusEnum.Opening;
                if (linkStatusCheckInterval < 100) linkStatusCheckInterval = 100;
                this.linkStatus.startCheck(linkStatusCheckInterval);
                this.autoLinkStatus = ServiceStatusEnum.Opened;
                this.setStatus = "�����Զ�����";
            }
            else
            {
                stopCheck();
            }
        }

        /// <summary>
        /// �ر��Զ����״̬���Զ�����
        /// </summary>
        public void stopCheck()
        {
            if (reader == null) return;
            if (this.linkStatus.isChecking() == true)
            {
                this.autoLinkStatus = ServiceStatusEnum.Closing;
                this.linkStatus.stopCheck();
                this.autoLinkStatus = ServiceStatusEnum.Closed;
            }
            this.setStatus = "ֹͣ�Զ�����";
        }

        /// <summary>
        /// ʹ�õײ�⺯���������״̬(δʹ�ã�
        /// </summary>
        /// <returns></returns>
        public LinkStatusEnum _getLinkStatus()
        {
            if (reader == null) return LinkStatusEnum.Unknown;

            //             switch (reader.ConnectStatus)
            //             {
            //                 case ConnectStatusEnum.CONNECTED:
            //                     return LinkStatusEnum.Linked;
            //                 case ConnectStatusEnum.DISCONNECTED:
            //                     return LinkStatusEnum.Unlink;
            //                 case ConnectStatusEnum.CONNECTING:
            //                     return LinkStatusEnum.Linking;
            //                 case ConnectStatusEnum.CONNECTLOST:
            //                     return LinkStatusEnum.Unlink;
            //                 default:
            //                     return LinkStatusEnum.Unknown;
            //             }
            return LinkStatusEnum.Unknown;
        }

        /// <summary>
        /// ��ȡ����״̬����
        /// </summary>
        /// <returns></returns>
        public LinkStatusEnum getLinkStatus()
        {
            if (reader == null) return LinkStatusEnum.Unknown;
            TimeCounter tc = new TimeCounter();

            try
            {
                tc.start();
                TagInfo info = new TagInfo();
                if (reader.CheckStatus() == Status_enum.SUCCESS)
                {
                    //                     this.setStatus = "�ɹ�" + tc.ToString() + " ����";
                    return LinkStatusEnum.Linked;
                }
                else
                {
                    //                     this.setStatus = "ʧ��" + tc.ToString() + " ����";
                    return LinkStatusEnum.Unlink;
                }
            }
            catch
            {
                //                 this.setStatus = "ʧ��" + tc.ToString() + " ����";
                return LinkStatusEnum.Unlink;
            }




            //             TimeCounter tc = new TimeCounter();
            //             tc.start();
            // 
            // 
            //             byte[] ver = new byte[20];
            //             byte len = (byte)ver.Length;
            //             for (int i = 0; i < len; i++) { ver[i] = 0x00; }
            // 
            //             try
            //             {
            //                 Status_enum stat = reader.GetSWVer((byte)this.readerID, ref ver, ref len);
            //                 if (stat == Status_enum.SUCCESS)
            //                 {
            //                     this.setStatus = "�ɹ�" + tc.ToString() + " ����";
            // 
            //                     return LinkStatusEnum.Linked;
            //                 }
            //                 else
            //                 {
            // //                     this.linkDisconnectionCount++;
            //                     this.setStatus = "ʧ��" + tc.ToString() + " ����";
            // 
            //                     return LinkStatusEnum.Unlink;
            //                 }
            //             }
            //             catch
            //             {
            // //                 this.linkDisconnectionCount++;
            //                 this.setStatus = "ʧ��" + tc.ToString() + " ����";
            // 
            //                 return LinkStatusEnum.Unlink;
            //             }
        }

        #endregion

        #region ��д��ǩ����

        /// <summary>
        /// ��ǩ��дѰַ��ʽ
        /// </summary>
        private TagAddressionModeEnum tagAddressingMode = default(TagAddressionModeEnum);

        /// <summary>
        /// �������ı�ǩ��UID
        /// </summary>
        private string lastTagUID = "";

        /// <summary>
        /// ���һ�����õı�ǩ�û��ڴ�������ʼ��ַ
        /// </summary>
        private int lastTagDataStartBlock = 0;

        /// <summary>
        /// ���һ�����õı�ǩ�û��ڴ����ݿ�����
        /// </summary>
        private int lastTagDataBlockQuantity = 1;

        /// <summary>
        /// ���һ�����õı�ǩ�û��ڴ����ݿ��С
        /// </summary>
        private TagBlockSizeEnum lastTagDataBlockSize = default(TagBlockSizeEnum);

        /// <summary>
        /// ���һ�ζ�/д���ı�ǩ�û��ڴ�����
        /// </summary>
        public string lastTagData;

        #endregion

        #region ��д��ǩ����

        /// <summary>
        /// ��ȡ��ǩUID
        /// </summary>
        public void readTagUID()
        {
            byte[] uid = new byte[8];
            if (reader.Inventory((byte)this.readerID, ref uid) != Status_enum.SUCCESS)
            {
                return;
            }

            HexString hs = new HexString(uid);
            if (hs.hex == null)
            {
                return;
            }
            this.lastTagUID = hs.ToString();
        }

        /// <summary>
        /// ����ǩ�û��ڴ�
        /// </summary>
        public void readTagMemory()
        {
            byte[] data = new byte[160];
            byte len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadSBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, 0x00, ref data, ref len))
            {
                //                 this.setStatus("�����ʧ��");
                return;
            }

            if (len == 4)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            byte start = (byte)this.lastTagDataStartBlock;
            //����8��
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8;
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "�����ʧ��";
                return;
            }
            HexString hs = new HexString(data, len);
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
            this.setStatus = "�����ɹ�";
        }

        /// <summary>
        /// д��ǩ�û��ڴ�
        /// </summary>
        public void writeTagMemory()
        {
            byte[] data = new byte[16];
            byte len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadSBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, 0x00, ref data, ref len))
            {
                return;
            }

            if (len == 4)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            byte start = (byte)this.lastTagDataStartBlock;
            //����8��
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8;
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            int blockSize = (int)this.lastTagDataBlockSize;
            int byteLen = ((int)cnt * blockSize);
            HexString hs = new HexString(this.lastTagData, byteLen);
            if (hs.len() != byteLen)
            {
                return;
            }

            else if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, hs.hex))
            {
                return;
            }
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
        }

        #endregion

        #region ��ǩ��ȫ����Ϣ����

        /// <summary>
        /// ���һ�λ�õı�ǩ���С��Ϣ
        /// </summary>
        private int _lastTagBlockSize = 0;
        public int lastTagBlockSize
        {
            get { return _lastTagBlockSize; }
            set { setValue(ref _lastTagBlockSize, value, "lastTagBlockSize"); }
        }

        /// <summary>
        /// ���һ�λ�õı�ǩ��������Ϣ
        /// </summary>
        private int _lastTagBlockQuantity = 0;
        public int lastTagBlockQuantity
        {
            get { return _lastTagBlockQuantity; }
            set { setValue(ref _lastTagBlockQuantity, value, "lastTagBlockQuantity"); }
        }

        #endregion

        #region ��ǩ��ȫ����Ϣ����

        public void readTagSystemInfo()
        {
            TagInfo info = new TagInfo();
            if (Status_enum.SUCCESS != reader.GetTagInfo((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, ref info, Antenna_enum.ANT_1))
            {
                return;
            }

            this.lastTagBlockSize = (int)info.BlockSize + 1;
            if ((int)info.BlockSize == 3)
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_4_Bytes;
            }
            else
            {
                this.lastTagDataBlockSize = TagBlockSizeEnum.Size_8_Bytes;
            }

            this.lastTagBlockQuantity = (int)info.BlockCnt + 1;
            this.lastTagDataBlockQuantity = (int)info.BlockCnt + 1;
        }

        public void readTagSecurityInfo()
        {
            byte start = (byte)this.lastTagDataStartBlock;
            if (this.lastTagDataBlockQuantity > 8) this.lastTagDataBlockQuantity = 8; //����8��
            byte cnt = (byte)this.lastTagDataBlockQuantity;
            int blockSize = (int)this.lastTagDataBlockSize;
            int byteLen = ((int)cnt * blockSize);
            byte[] data = new byte[byteLen];

            if (Status_enum.SUCCESS != reader.GetTagSecurity((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data))
            {
                return;
            }

            HexString hs = new HexString(data, byteLen);
            this.lastTagData = hs.ToString((int)this.lastTagDataBlockSize);
        }

        #endregion


        #region ��ǩ������

        /// <summary>
        /// ��ǩ�ڴ�����
        /// </summary>
        private string _lastTagMemory = "";
        public string lastTagMemory
        {
            get { return _lastTagMemory; }
            set { setValue(ref _lastTagMemory, value, "lastTagMemory"); }
        }

        public void tagAnalyserRead()
        {
            byte[] data = new byte[160];
            byte len = (byte)data.Length;

            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            this.lastTagMemory = "";
            int i = 0;
            byte start;
            byte cnt;
            HexString hs;

            TimeCounter tc = new TimeCounter();
            tc.start();

            //��8��Ϊ��λ����������
            for (i = 0; i < blockCount / 8; i++)
            {
                start = (byte)(i * 8);
                cnt = (byte)8;
                len = (byte)data.Length;
                if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                {
                    this.setStatus = "���ݶ���ʧ��";
                    return;
                }
                hs = new HexString(data, len);
                this.lastTagMemory += hs.ToString(blockSize);
            }

            //����ʣ���
            start = (byte)(i * 8);
            cnt = (byte)(blockCount % 8);
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "���ݶ���ʧ��";
                return;
            }
            hs = new HexString(data, len);
            this.lastTagMemory += hs.ToString(blockSize);

            this.setStatus = "�����ɹ�����ʱ " + tc.ToString() + " ����";
            return;
        }

        public void tagAnalyserWrite()
        {
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity; //������
            int blockSize = this.lastTagBlockSize; //���С

            int i = 0;
            byte start;
            byte cnt;
            HexString hs;

            TimeCounter tc = new TimeCounter();
            tc.start();

            hs = new HexString(this.lastTagMemory);
            byte[] data;

            int writeBlockCount = hs.len() / blockSize;
            bool dataCutFlag = false;
            if ((hs.len() % blockSize) != 0 || writeBlockCount > blockCount)
            {
                dataCutFlag = true;
            }

            if (writeBlockCount > 0)
            {
                for (i = 0; i < writeBlockCount / 8; i++)
                {
                    start = (byte)(i * 8);
                    cnt = (byte)8;
                    data = hs.getHex(start * blockSize, cnt * blockSize);
                    if (data == null)
                    {
                        this.setStatus = "���ݴ���д��ʧ��";
                        return;
                    }

                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                    {
                        this.setStatus = "���ݴ���д��ʧ��";
                        return;
                    }
                }

                start = (byte)(i * 8);
                cnt = (byte)(writeBlockCount % 8);
                data = hs.getHex(start * blockSize, cnt * blockSize);
                if (data == null)
                {
                    this.setStatus = "���ݴ���д��ʧ��";
                    return;
                }

                if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                {
                    this.setStatus = "���ݴ���д��ʧ��";
                    return;
                }
            }
            else
            {
                this.setStatus = "���ݹ��̻��ʽ����д��ʧ��";
                return;
            }

            if (dataCutFlag == true)
            {
                this.setStatus = "����ĩβ���ض�,д��ɹ�,��ʱ " + tc.ToString() + " ����";
            }
            else
            {
                this.setStatus = "д��ɹ�,��ʱ " + tc.ToString() + " ����";
            }

        }

        #endregion

        #region ��ǩ������ASCII

        /// <summary>
        /// ��ǩ�ڴ�����ASCII��ʽ
        /// </summary>
        private string _lastTagMemoryASCII = "";
        public string lastTagMemoryASCII
        {
            get { return _lastTagMemoryASCII; }
            set { setValue(ref _lastTagMemoryASCII, value, "lastTagMemoryASCII"); }
        }

        public void tagAnalyserReadASCII()
        {
            byte[] data = this.tagReadAll();
            HexASCII ha = new HexASCII(data);
            lastTagMemoryASCII = ha.ToString();
        }

        public void tagAnalyserWriteASCII()
        {
            HexASCII ha = new HexASCII(lastTagMemoryASCII);
            this.tagWriteAll(ha.hex);
        }

        /// <summary>
        /// ��ȡ���б�ǩ����
        /// </summary>
        /// <returns>��ǩ����byte[]</returns>
        public byte[] tagReadAll()
        {
            linkStatus.threakeepCheckEventOK = false;

            byte[] data = new byte[160];
            byte len = (byte)data.Length;

            //��ʱ
            TimeCounter tc = new TimeCounter();
            tc.start();

            //��ȡ��ǩ���С�������
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            //���ص����ݣ���ʼ��
            byte[] tagMemory = new byte[blockCount * blockSize];
            for (int j = 0; j < tagMemory.Length; j++)
            {
                tagMemory[j] = 0x00;
            }

            int i = 0;
            byte start;
            byte cnt;

            //��8��Ϊ��λ����������
            for (i = 0; i < blockCount / 8; i++)
            {
                start = (byte)(i * 8);
                cnt = (byte)8;
                len = (byte)data.Length;
                if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                {
                    this.setStatus = "���ݶ���ʧ��";
                    linkStatus.threakeepCheckEvent.Set();
                    return tagMemory;
                }
                Array.Copy(data, 0, tagMemory, start * blockSize, len);
            }

            //����ʣ���
            start = (byte)(i * 8);
            cnt = (byte)(blockCount % 8);
            len = (byte)data.Length;
            if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
            {
                this.setStatus = "���ݶ���ʧ��";
                return tagMemory;
            }
            Array.Copy(data, 0, tagMemory, start * blockSize, len);
            this.setStatus = "�����ɹ�:" + tc.ToString() + " ����";
            linkStatus.threakeepCheckEvent.Set();
            return tagMemory;
        }

        /// <summary>
        /// д����������
        /// </summary>
        /// <param name="tagMemory">��Ҫд�������byte[]</param>
        public void tagWriteAll(byte[] tagMemory)
        {
            //��ʱ
            TimeCounter tc = new TimeCounter();
            tc.start();

            //��ȡ��ǩ���С�������
            this.readTagUID();
            this.readTagSystemInfo();
            int blockCount = this.lastTagBlockQuantity;
            int blockSize = this.lastTagBlockSize;

            int i = 0;
            byte start;
            byte cnt;

            byte[] data = new byte[blockSize * 8];

            int writeBlockCount = tagMemory.Length / blockSize;
            bool dataCutFlag = false;

            //�ǿ��С����
            if ((tagMemory.Length % blockSize) != 0)
            {
                writeBlockCount = writeBlockCount + 1;
            }

            //��������
            if (writeBlockCount > blockCount)
            {
                dataCutFlag = true;
                writeBlockCount = blockCount;
            }

            //��ʼд��
            if (writeBlockCount > 0)
            {
                for (i = 0; i < writeBlockCount / 8; i++)
                {
                    start = (byte)(i * 8);
                    cnt = (byte)8;
                    Array.Copy(tagMemory, start * blockSize, data, 0, cnt * blockSize);

                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                    {
                        this.setStatus = "���ݴ���д��ʧ��";
                        return;
                    }
                }

                //д��ʣ���
                start = (byte)(i * 8);
                cnt = (byte)(writeBlockCount % 8);
                data = new byte[cnt * blockSize];
                for (int j = 0; j < data.Length; j++)
                {
                    if (j + start * blockSize < tagMemory.Length)
                    {
                        data[j] = tagMemory[j + start * blockSize];
                    }
                    else
                    {
                        data[j] = 0x00;
                    }
                }

                if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, blockSize, data))
                {
                    this.setStatus = "���ݴ���д��ʧ��";
                    return;
                }
            }
            else
            {
                this.setStatus = "���ݹ��̻��ʽ����д��ʧ��";
                return;
            }

            if (dataCutFlag == true)
            {
                this.setStatus = "����ĩβ���ض�,д��ɹ�,��ʱ " + tc.ToString() + " ����";
            }
            else
            {
                this.setStatus = "д��ɹ�,��ʱ " + tc.ToString() + " ����";
            }
        }

        public bool ReadUserMessage(byte DataLenth, ref byte[] tagMemory)
        {
            linkStatus.threakeepCheckEventOK = false;

            //��ʱ
            TimeCounter tc = new TimeCounter();
            tc.start();

            //             byte[] tagMemory = new byte[DataLenth];
            for (int jj = 0; jj < DataLenth; jj++)
            {
                tagMemory[jj] = 0x00;
            }

            byte blockSize = 4;
            byte start = 0;
            byte cnt = (byte)8;
            byte len = (byte)(blockSize * cnt);
            byte[] data = new byte[len];
            for (int ii = 0; ii < DataLenth; ii += blockSize * cnt)
            {
                start = (byte)(ii / blockSize);
                if ((ii + len) <= DataLenth)
                {
                    if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                    {
                        this.setStatus = "���ݶ���ʧ��!";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                else
                {
                    cnt = (byte)((DataLenth - ii) / blockSize);
                    len = (byte)(blockSize * cnt);
                    data = new byte[len];
                    if (Status_enum.SUCCESS != reader.ReadMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, start, cnt, ref data, ref len))
                    {
                        this.setStatus = "���ݶ���ʧ��!";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                Array.Copy(data, 0, tagMemory, start * 4, len);
            }
            this.setStatus = "�����ɹ�:" + tc.ToString() + " ����";
            linkStatus.threakeepCheckEvent.Set();
            return true;
        }
        public bool WriteUserMessage(int addresstart, byte[] Data)
        {
            linkStatus.threakeepCheckEventOK = false;
            //��ʱ
            TimeCounter tc = new TimeCounter();
            tc.start();

            byte blockSize = 4;
            byte cnt = (byte)8;
            if ((Data.Length % blockSize) != 0)//ֻ��д��������
            {
                this.setStatus = "���ݴ���" + tc.ToString() + " ����";
                return false;
            }
            byte len = (byte)(blockSize * cnt);
            byte Star = (byte)(addresstart / blockSize);
            for (int ii = 0; ii < Data.Length; ii += len)
            {
                Star = (byte)(Star + (ii / blockSize));
                if ((ii + len) <= Data.Length)
                {
                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, Star, cnt, blockSize, Data))
                    {
                        this.setStatus = "д��ʧ��" + tc.ToString() + " ����";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
                else
                {
                    cnt = (byte)((Data.Length - ii) / blockSize);
                    //                     len = (byte)(4 * cnt);
                    if (Status_enum.SUCCESS != reader.WriteMBlock((byte)this.readerID, Opcode_enum.NON_ADDRESS_MODE, null, Star, cnt, blockSize, Data))
                    {
                        this.setStatus = "д��ʧ��" + tc.ToString() + " ����";
                        linkStatus.threakeepCheckEvent.Set();
                        return false;
                    }
                }
            }
            this.setStatus = "д��ɹ�" + tc.ToString() + " ����";
            linkStatus.threakeepCheckEvent.Set();
            return true;
        }


        #endregion


        #region �Զ�����

        /// <summary>
        /// �Զ�����״̬
        /// </summary>
        private ServiceStatusEnum _autoLinkStatus = default(ServiceStatusEnum);
        public ServiceStatusEnum autoLinkStatus
        {
            get { return _autoLinkStatus; }
            set { setValue(ref _autoLinkStatus, value, "autoLinkStatus"); }
        }

        /// <summary>
        /// ����״̬�ı��¼�������
        /// </summary>
        /// <param name="sender"></param>
        public void linkStatusChangeHandler(StatusVM<LinkStatusEnum> sender)
        {
            PLC.HNC8PLCRFIDEvent m_SendData = new PLC.HNC8PLCRFIDEvent();
            m_SendData.RFIDserial = this.RFIDserial;
            if (sender.now == LinkStatusEnum.Unlink)
            {
                m_SendData.EventType = -1;
            }
            else if (sender.now == LinkStatusEnum.Linked)
            {
                m_SendData.EventType = 0;
            }

            if (RFID.AutoSendHandler != null)
            {
                RFID.AutoSendHandler.BeginInvoke(this, m_SendData, null, null);
            }
            if (StateChangeHandler != null)
            {
                StateChangeHandler.BeginInvoke(this, m_SendData, null, null);
            }

            //����������Զ�������ֱ�ӷ���
            //             if (this.autoLinkStatus != ServiceStatusEnum.Opened)
            //             {
            //                 return;
            //             }

            //���״̬����������Ҫ����
            //             if (LinkStatusEnum.Linked != sender.now && LinkStatusEnum.Unlinking != sender.now && LinkStatusEnum.Linked == sender.last)
            //             if (LinkStatusEnum.Linked != sender.now)
            //             {
            //                 try
            //                 {
            //                     this.connect();
            //                 }
            //                 catch { ;}
            //                 this.linkStatus.update();
            //             }
        }


        #endregion

        #region RFIDReadDataDataTable��XML��ת
        /// <summary>
        /// ��DataTable������д�뵽XML�ļ���
        /// </summary>
        /// <param name="dt">����Դ</param>
        /// <param name="address">XML�ļ���ַ</param>
//         public static bool WriteToXml(System.Data.DataTable dt, string address)
//         {
//             try
//             {
//                 //����ļ�DataTable.xml������ֱ��ɾ��
//                 if (System.IO.File.Exists(address))
//                 {
//                     System.IO.File.Delete(address);
//                 }
// 
//                 System.Xml.XmlTextWriter writer =
//                     new System.Xml.XmlTextWriter(address, Encoding.GetEncoding("GBK"));
//                 writer.Formatting = System.Xml.Formatting.Indented;
// 
//                 //XML�ĵ�������ʼ
//                 writer.WriteStartDocument();
// 
//                 writer.WriteComment("DataTable: " + dt.TableName);
// 
//                 writer.WriteStartElement("DataTable"); //DataTable��ʼ
// 
//                 writer.WriteAttributeString("TableName", dt.TableName);
//                 writer.WriteAttributeString("CountOfRows", dt.Rows.Count.ToString());
//                 writer.WriteAttributeString("CountOfColumns", dt.Columns.Count.ToString());
// 
//                 writer.WriteStartElement("ClomunName", ""); //ColumnName��ʼ
// 
//                 for (int i = 0; i < dt.Columns.Count; i++)
//                 {
//                     writer.WriteAttributeString(
//                         "Column" + i.ToString(), dt.Columns[i].ColumnName);
//                 }
// 
//                 writer.WriteEndElement(); //ColumnName����
// 
//                 //���и���
//                 for (int j = 0; j < dt.Rows.Count; j++)
//                 {
//                     writer.WriteStartElement("Row" + j.ToString(), "");
// 
//                     //��ӡ����
//                     for (int k = 0; k < dt.Columns.Count; k++)
//                     {
//                         writer.WriteAttributeString(
//                             "Column" + k.ToString(), dt.Rows[j][k].ToString());
//                     }
// 
//                     writer.WriteEndElement();
//                 }
// 
//                 writer.WriteEndElement(); //DataTable����
// 
//                 writer.WriteEndDocument();
//                 writer.Close();
// 
//                 //XML�ĵ���������
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//                 return false;
//             }
// 
//             return true;
//         }
// 
//         /// <summary>
//         /// ��XML�ļ��ж�ȡһ��DataTable
//         /// </summary>
//         /// <param name="dt">����Դ</param>
//         /// <param name="address">XML�ļ���ַ</param>
//         /// <returns></returns>
//         public System.Data.DataTable ReadFromXml(string address)
//         {
//             System.Data.DataTable dt = new System.Data.DataTable();
// 
//             try
//             {
//                 if (!System.IO.File.Exists(address))
//                 {
//                     throw new Exception("�ļ�������!");
//                 }
// 
//                 System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
//                 xmlDoc.Load(address);
// 
//                 System.Xml.XmlNode root = xmlDoc.SelectSingleNode("DataTable");
// 
//                 //��ȡ����
//                 dt.TableName = ((System.Xml.XmlElement)root).GetAttribute("TableName");
//                 //Console.WriteLine("��ȡ������ {0}", dt.TableName);
// 
//                 //��ȡ����
//                 int CountOfRows = 0;
//                 if (!int.TryParse(((System.Xml.XmlElement)root).
//                     GetAttribute("CountOfRows").ToString(), out CountOfRows))
//                 {
//                     throw new Exception("����ת��ʧ��");
//                 }
// 
//                 //��ȡ����
//                 int CountOfColumns = 0;
//                 if (!int.TryParse(((System.Xml.XmlElement)root).
//                     GetAttribute("CountOfColumns").ToString(), out CountOfColumns))
//                 {
//                     throw new Exception("����ת��ʧ��");
//                 }
// 
//                 //�ӵ�һ���ж�ȡ��¼������
//                 foreach (System.Xml.XmlAttribute xa in root.ChildNodes[0].Attributes)
//                 {
//                     dt.Columns.Add(xa.Value);
//                     //Console.WriteLine("�����У� {0}", xa.Value);
//                 }
// 
//                 //�Ӻ�������ж�ȡ����Ϣ
//                 for (int i = 1; i < root.ChildNodes.Count; i++)
//                 {
//                     string[] array = new string[root.ChildNodes[0].Attributes.Count];
//                     for (int j = 0; j < array.Length; j++)
//                     {
//                         array[j] = root.ChildNodes[i].Attributes[j].Value.ToString();
//                     }
//                     dt.Rows.Add(array);
//                     //Console.WriteLine("�в���ɹ�");
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//                 return new System.Data.DataTable();
//             }
// 
//             return dt;
//         }

        #endregion
    }
    #endregion

    #region StatusVM<STATUS_TYPE>

    /// <summary>
    /// ʵʱ״̬��ʾVM
    /// </summary>
    /// <typeparam name="STATUS_TYPE">״̬�������ͣ���ö�ٻ������ȣ�</typeparam>
    /// <bug>
    ///  1 - û��ʵ������������ɾ�������ǽ����ֶ���������������رն�����ִ�кܳ�ʱ�䣨�ȴ��̱߳�����
    ///  2 - ״̬���¶�ȡ��ԭ����û�б�֤
    /// </bug>
    /// <example>
    /// in xaml:
    ///     <Window.Resources>
    ///         <Style x:Key="StatusStyle" TargetType="Shape">
    ///             <Style.Triggers>
    ///                 <DataTrigger Binding="{Binding Path=s.now}" Value="{x:Static vm:LightStatus.ON}">
    ///                     <Setter Property="Fill" Value="Green" />
    ///                 </DataTrigger>
    ///                 <DataTrigger Binding="{Binding Path=s.now}" Value="{x:Static vm:LightStatus.OFF}">
    ///                     <Setter Property="Fill" Value="#FF6666" />
    ///                 </DataTrigger>
    ///             </Style.Triggers>
    ///         </Style>
    ///     </Window.Resources>
    /// </example>
    public class StatusVM<STATUS_TYPE> : ObservableObject //IDisposable
    {
        public StatusVM(object owner, Func<STATUS_TYPE> getStatusFunc)
        {
            this.owner = owner;
            this.getStatusFunc = getStatusFunc;
        }

        ~StatusVM()
        {
            this.stopCheck();
        }

        #region ����

        /// <summary>
        /// �Զ������߳�Handle
        /// </summary>
        private Thread keepCheckThread = null;

        /// <summary>
        /// ֹͣ�Զ������߳�Flag
        /// </summary>
        private bool StopCheckFlag = true;

        /// <summary>
        /// �Զ����¼�������룩
        /// </summary>
        private int checkInterval_ms = 0;

        /// <summary>
        /// ״̬����¼� function(object this)
        /// �磺public void linkStatusChangeHandler(StatusVM<LinkStatusEnum> sender);
        /// </summary>
        public event Action<StatusVM<STATUS_TYPE>> StatusChanged;
        public event Action<StatusVM<STATUS_TYPE>> LinkEvent;
        //         public delegate void RFIDEventHandler<PlcEvent>(object sender, MITSUBISHIPLCRFIDEvent Args);
        // 
        //         public event EventHandler<PlcEvent> AutoSendHandler;

        /// <summary>
        /// ״̬ӵ����
        /// </summary>
        public object owner;

        /// <summary>
        /// �Զ����״̬���º���
        /// </summary>
        public Func<STATUS_TYPE> getStatusFunc;

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        private STATUS_TYPE _now = default(STATUS_TYPE);
        public STATUS_TYPE now
        {
            get { return _now; }
            set
            {
                if (!object.Equals(_now, value))
                {
                    _last = _now;
                    _now = value;

                    if (this.StatusChanged != null)
                    {
                        this.StatusChanged.BeginInvoke(this, new AsyncCallback(this.EventFinish), "StatusChangedFinish");
                    }

                    OnPropertyChanged("now");
                    OnPropertyChanged("last");
                }
                if (this.LinkEvent != null)
                {
                    //this.LinkEvent.Invoke(this);//��������ѭ��Ƕ������
                    this.LinkEvent.BeginInvoke(this, new AsyncCallback(this.EventFinish), "LinkEventFinish");
                }

            }
        }


        private void EventFinish(IAsyncResult result)
        {
            Action<StatusVM<STATUS_TYPE>> Handler = (Action<StatusVM<STATUS_TYPE>>)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
            Handler.EndInvoke(result);
            Console.WriteLine(result.AsyncState.ToString());
        }

        /// <summary>
        /// ��һʱ�̵�״̬
        /// </summary>
        private STATUS_TYPE _last = default(STATUS_TYPE);
        public STATUS_TYPE last
        {
            get { return _last; }
        }

        #endregion

        #region ����״̬����

        /// <summary>
        /// ����״̬����
        /// </summary>
        /// <returns></returns>
        public STATUS_TYPE update()
        {
            STATUS_TYPE tmp;
            //             lock (this._now)
            {
                this.now = this.getStatusFunc();
            }
            return this.now;
        }

        public void reset()
        {
            //д������now,��ˢ��last
            this.now = default(STATUS_TYPE);
            this.now = default(STATUS_TYPE);
        }

        #endregion

        #region �Զ�����״̬�̷߳���

        /// <summary>
        /// �Զ�����״̬�̺߳���
        /// </summary>
        public System.Threading.AutoResetEvent threakeepCheckEvent = new System.Threading.AutoResetEvent(true);
        public bool threakeepCheckEventOK = true;
        private void keepCheck()
        {

            while (!StopCheckFlag)
            {
                if (threakeepCheckEventOK)
                {
                    this.update();
                }
                else
                {
                    threakeepCheckEvent.WaitOne();
                    threakeepCheckEventOK = true;
                }
                Thread.Sleep(this.checkInterval_ms);
            }
        }

        /// <summary>
        /// �����Զ�����״̬�߳�
        /// </summary>
        public void startCheck(int checkInterval_ms = 3000)
        {
            if (this.keepCheckThread == null)
            {
                this.checkInterval_ms = checkInterval_ms;
                this.StopCheckFlag = false;
                this.keepCheckThread = new Thread(this.keepCheck);
                this.keepCheckThread.IsBackground = true;
                this.keepCheckThread.Start();
            }

        }

        /// <summary>
        /// ֹͣ�Զ�����״̬�߳�
        /// </summary>
        public void stopCheck()
        {
            if (this.keepCheckThread != null && this.keepCheckThread.IsAlive)
            {
                this.StopCheckFlag = true;
                this.keepCheckThread.Join();
                this.keepCheckThread = null;
                this.checkInterval_ms = 0;
            }
        }

        /// <summary>
        /// �Ƿ��Ѿ������Զ����
        /// </summary>
        /// <returns></returns>
        public bool isChecking()
        {
            return !(this.StopCheckFlag);
        }

        #endregion

        public override string ToString()
        {
            return this.now.ToString();
        }
    }

    #endregion

    #region SxReaderVMl��
    //     public class SxReaderVM : ObservableObject
    //     {
    //         public SxReaderVM()
    //         {
    //             this.messages = new AsyncObservableCollection<MessageVM>();
    //         }
    // 
    //         /// <summary>
    //         /// ��д������
    //         /// </summary>
    //         private ReaderTypeEnum _readerType = default(ReaderTypeEnum);
    //         public ReaderTypeEnum readerType
    //         {
    //             get { return _readerType; }
    //             set { setValue(ref _readerType, value, "readerType"); }
    //         }
    // 
    //         /// <summary>
    //         /// ��д��ID
    //         /// </summary>
    //         private int _readerID = 0;
    //         public int readerID
    //         {
    //             get { return _readerID; }
    //             set { setValue(ref _readerID, value, "readerID"); }
    //         }
    // 
    //         /// <summary>
    //         /// ���ӵ�ַ�ַ���
    //         /// </summary>
    //         private string _linkAddress = "";
    //         public string linkAddress
    //         {
    //             get { return _linkAddress; }
    //             set { setValue(ref _linkAddress, value, "linkAddress"); }
    //         }
    // 
    //         private string _linkArgument = "";
    //         public string linkArgument
    //         {
    //             get { return _linkArgument; }
    //             set { setValue(ref _linkArgument, value, "linkArgument"); }
    //         }
    // 
    //         /// <summary>
    //         /// ���ӽ�����ʽ
    //         /// </summary>
    //         private LinkModeEnum _linkMode = default(LinkModeEnum);
    //         public LinkModeEnum linkMode
    //         {
    //             get { return _linkMode; }
    //             set { setValue(ref _linkMode, value, "linkMode"); }
    //         }
    // 
    //         /// <summary>
    //         /// ��������״̬
    //         /// </summary>
    //         private string _runTimeStatus = "";
    //         public string runTimeStatus
    //         {
    //             get { return _runTimeStatus; }
    //             set { setValue(ref _runTimeStatus, value, "runTimeStatus"); }
    //         }
    // 
    //         /// <summary>
    //         /// �����³�������״̬
    //         /// </summary>
    //         /// <param name="stat"></param>
    //         /// <param name="level"></param>
    //         /// <param name="sender"></param>
    //         public void setStatus(string stat, int level = 0, string sender = "")
    //         {
    //             this.runTimeStatus = stat;
    //             //addMessage(stat,level,sender); //����������
    //         }
    // 
    //         #region ��Ϣ�б�����
    // 
    //         /// <summary>
    //         /// ��Ϣ�б�
    //         /// </summary>
    //         public AsyncObservableCollection<MessageVM> messages { get; set; }
    // 
    //         /// <summary>
    //         /// ��Ϣ�б������Ϣ����0Ϊ�����ƣ�
    //         /// </summary>
    //         private int _maxMessagesLength = 10;
    //         public int maxMessagesLength
    //         {
    //             get { return _maxMessagesLength; }
    //             set { setValue(ref _maxMessagesLength, value, "maxMessagesLength"); }
    //         }
    // 
    //         #endregion
    // 
    //         #region ��Ϣ�б���
    // 
    //         public void addMessage(string message, int level = 0, string sender = "")
    //         {
    //             MessageVM msg = new MessageVM(message, level, sender);
    //             this.addMessage(msg);
    //         }
    // 
    //         public void addMessage(MessageVM msg)
    //         {
    //             lock (this.messages)
    //             {
    //                 if (this.maxMessagesLength > 0 && this.messages.Count() >= this.maxMessagesLength)
    //                 {
    //                     while (this.messages.Count() >= this.maxMessagesLength)
    //                     {
    //                         this.messages.RemoveAt(0);
    //                     }
    //                 }
    //                 this.messages.Add(msg);
    //             }
    //         }
    // 
    //         #endregion
    //     }
    #endregion

    #region ���ӻ����󣨰�UIԪ�����ݣ�����ʱ�Զ�ˢ��UI��
    /// <summary>
    /// ���ӻ����󣨰�UIԪ�����ݣ�����ʱ�Զ�ˢ��UI��
    /// </summary>

    public class ObservableObject : INotifyPropertyChanged
    {

        protected void setValue<T>(ref T property, T value, string propertyName)
        {
            if (object.Equals(property, value))
                return;

            property = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public class AsyncObservableObject : INotifyPropertyChanged
    {

        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        protected void setValue<T>(ref T property, T value, string propertyName)
        {
            if (object.Equals(property, value))
                return;

            property = value;
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (SynchronizationContext.Current == _synchronizationContext)
            {
                // Execute the PropertyChanged event on the current thread
                RaisePropertyChanged(propertyName);
            }
            else
            {
                // Raises the PropertyChanged event on the creator thread
                _synchronizationContext.Send(RaisePropertyChanged, propertyName);
            }
        }

        private void RaisePropertyChanged(object param)
        {
            // We are in the creator thread, call the base implementation directly
            _OnPropertyChanged((string)param);
        }

        protected void _OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    #endregion

    #region ����
    public class TimeCounter
    {
        DateTime startTime;//��ʼ

        public TimeCounter()
        {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// ��ʼ/���¿�ʼ��ʱ����¼��ʼʱ�䣩
        /// </summary>
        public void start()
        {
            startTime = DateTime.Now;
        }

        /// <summary>
        /// ���ؿ�ʼ����ǰ�ĺ�����
        /// </summary>
        /// <returns>��ʼ����ǰ�ĺ�����</returns>
        public double now()
        {
            return (DateTime.Now - startTime).TotalMilliseconds;
        }

        /// <summary>
        /// ���ؿ�ʼ����ǰ�ĺ��������ַ���
        /// </summary>
        /// <returns>��ʼ����ǰ�ĺ��������ַ���</returns>
        public override string ToString()
        {
            return (DateTime.Now - startTime).TotalMilliseconds.ToString("0");
        }
    }
    public class HexString
    {
        /// <summary>
        /// ʮ�������ַ�����������ž������������ֵ
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
            if (s == null || hexLen == null || hexLen * 2 > s.Length)
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
            //ȥ���ַ����еķǴ�дʮ�������ַ�
            for (int i = 0; i < hexstr.Length; i++)
            {
                if (hex_std.IndexOf(hexstr[i]) >= 0)
                {
                    s += hexstr[i];
                }
            }
            //����Ƿ�Ϊż�����ַ�
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
        /// ��ȡ����������
        /// </summary>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns>����������ȣ�����null</returns>
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

            //������鳤�Ȳ�����0��������Ϊÿ��byte������ո�
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
            if (s == null || hexLen == null || hexLen * 2 > s.Length)
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
    #endregion

    #region ˼�ȶ�д����������
    /// <summary>
    /// ��д�����ӷ�ʽ
    /// </summary>
    public enum LinkModeEnum
    {
        TCP_IP = 0,
        RS_232
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public enum LinkStatusEnum
    {
        Unlink = 0,
        Linked = 1,
        Unlinking = 2,
        Linking = 3,
        Unknown = 4
    }

    /// <summary>
    /// ��д������
    /// </summary>
    public enum ReaderTypeEnum
    {
        S6 = 6,
        S7 = 7
    }

    /// <summary>
    /// ����ģʽ
    /// </summary>
    public enum PowerModeEnum
    {
        Half = 0,
        Full = 1
    }

    /// <summary>
    /// ָ����Чʱ��
    /// </summary>
    public enum CommandModeEnum
    {
        UntilSuccess = 0,
        JustOnce = 1,
    }

    /// <summary>
    /// �Զ�����ǩͨѶģʽ
    /// </summary>
    public enum ServiceCommunicationModeEnum
    {
        RS_232 = 0,
        RS_485 = 1,
        TCP_IP = 2
    }

    /// <summary>
    /// Ĭ�϶�д��ǩ���С
    /// </summary>
    public enum TagBlockSizeEnum
    {
        Size_4_Bytes = 4,
        Size_8_Bytes = 8
    }

    /// <summary>
    /// ��ǩ����Ѱַ��ʽ
    /// </summary>
    public enum TagAddressionModeEnum
    {
        None = 0, //��ʹ��Ѱַ
        UID //ͨ��UIDѰַ
    }

    /// <summary>
    /// ����״̬���Զ�����ǩ�ȣ�
    /// </summary>
    public enum ServiceStatusEnum
    {
        Closed = 0,
        Opened,
        Closing,
        Opening
    }
    #endregion
}
