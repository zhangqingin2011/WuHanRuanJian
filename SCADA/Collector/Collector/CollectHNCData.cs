using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCADA;
using HNCAPI;
using System.Threading;
using ScadaHncData;
using HNC_MacDataService;
using System.Net.NetworkInformation;

namespace Collector
{
    public class CollectHNCData : CollectDeviceData, IDisposable
    {
        public LineDevice.CNC m_cnc;
        public HNCData hncdata;
        public bool bCollect;
        public Thread collectThread;

        private CollectSysData gatherSYS;
        private CollectChanData gatherCH;
        private CollectAxisData gatherAXIS;

        public CollectHNCData(ref LineDevice.CNC cnc, ref List<HNCData> hncdatalist)
        {
            gatherSYS = new CollectSysData();
            gatherCH = new CollectChanData();
            gatherAXIS = new CollectAxisData();
            hncdata = new HNCData();
            hncdatalist.Add(hncdata);
            m_cnc = cnc;
            m_cnc.HCNCShareData = hncdata;
            hncdata.sysData.addr.ip = cnc.ip;
            hncdata.sysData.addr.port = cnc.port;
            hncdata.sysData.deviceCode = cnc.BujianID;
            hncdata.sysData.dbNo = cnc.dbNo;// net_to_redis
            bCollect = true;
        }

        public bool GatherData()
        {
            int dbNo = hncdata.sysData.dbNo;

            bool result = false;
            result = gatherSYS.GatherData(hncdata.sysData);

            for (int ii = 0; ii < hncdata.chanDataLst.Count; ii++)
            {
                result = gatherCH.GatherData(hncdata.chanDataLst[ii], dbNo) || result;
            }
            for (int ii = 0; ii < hncdata.axlist.Count; ii++)
            {
                result = gatherAXIS.GatherData(hncdata.axlist[ii], dbNo) || result;
            }

//             foreach (ChannelData chdata in hncdata.chanDataLst)
//             {
//                 result = gatherCH.GatherData(chdata, hncdata.sysData.clientNo) || result;
//             }
            
//             foreach (AxisData axdata in hncdata.axlist)
//             {
//                 result = gatherAXIS.GatherData(axdata, hncdata.sysData.clientNo) || result;
//             }

            // 获取所有报警 net_to_redis 暂时注释掉
            RefreshAlaramData(this);

            return result;
        }//根据clientNo采集数据

        /// <summary>
        /// 从redis读取报警信息
        /// </summary>
        /// <param name="collecncdata"></param>
        private void RefreshAlaramData(CollectHNCData collecncdata)
        {
            lock (collecncdata.hncdata.currentAlarmList)
            {
                List<String> alarmArray = MacDataService.GetInstance().GetHashAllString(collecncdata.hncdata.sysData.dbNo, "Alarm:AlarmCurrent");// Alarm: AlarmHistory
                if (alarmArray != null)
                {
                    //collecncdata.hncdata.currentAlarmList.Clear();
                    deleteAlarmCurrentAlarmLst(collecncdata, alarmArray);
					bool connect = PingTestCNC(collecncdata.m_cnc.ip, 300);
                    if (connect)
                    {
                    foreach (String alarmStr in alarmArray)
                    {
                        AlarmCurrent alarm = Newtonsoft.Json.JsonConvert.DeserializeObject<AlarmCurrent>(alarmStr);
                        AlarmData alm = new AlarmData();
                        alm.alarmNo = alarm.AlarmNo;
                        alm.isOnOff = 1;
                        alm.alarmTxt = alarm.alarmText;
                        alm.time = DateTime.Now;
                        //alm.time = DateTime.Parse(alarm.BeginTime);
                        addAlarmCurrentAlarmLst(collecncdata.hncdata, alm);
                    }
                }
                    else
                    {
                        clrAlarmCurrentAlarmLst(collecncdata, alarmArray);
                       
                    }
    
                }
            }
        }

        private static void clrAlarmCurrentAlarmLst(CollectHNCData collecncdata, List<String> alarmArray)//将当前的报警列表清除
        {
            for (Int32 i = 0; i < collecncdata.hncdata.currentAlarmList.Count; i++)
            {
                AlarmData alm = new AlarmData();
                
                //if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                    {
                        EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
                        alm.alarmNo = collecncdata.hncdata.currentAlarmList[i].alarmNo;
                        alm.isOnOff = 0;
                        alm.alarmTxt = collecncdata.hncdata.currentAlarmList[i].alarmTxt;
                        alm.time = DateTime.Now;
                        SendMeg.NeedFindTeX = false;
                        SendMeg.BujianID = collecncdata.m_cnc.BujianID;
                        SendMeg.alardat.isOnOff = 0; //更新报警栏取消的报警
                        SendMeg.alardat = alm;
                        //MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                    }
                    collecncdata.hncdata.currentAlarmList.RemoveAt(i);
            }
        }
        private static void deleteAlarmCurrentAlarmLst(CollectHNCData collecncdata, List<String> alarmArray)//将当前的报警列表删除更新后没有的报警
        {
            for (Int32 i = 0; i < collecncdata.hncdata.currentAlarmList.Count; i++)
            {
                bool find = false;
                AlarmData alm = new AlarmData();
                foreach (String alarmStr in alarmArray)
                {
                    AlarmCurrent alarm = Newtonsoft.Json.JsonConvert.DeserializeObject<AlarmCurrent>(alarmStr);
                    if (collecncdata.hncdata.currentAlarmList[i].alarmNo == alarm.AlarmNo
                            && collecncdata.hncdata.currentAlarmList[i].alarmTxt == alarm.alarmText
                            && collecncdata.hncdata.currentAlarmList[i].isOnOff == 1)
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    //if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                    {
                        EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
                        alm.alarmNo = collecncdata.hncdata.currentAlarmList[i].alarmNo;
                        alm.isOnOff = 0;
                        alm.alarmTxt = collecncdata.hncdata.currentAlarmList[i].alarmTxt;
                        alm.time = DateTime.Now;
                        SendMeg.NeedFindTeX = false;
                        SendMeg.BujianID = collecncdata.m_cnc.BujianID;
                        SendMeg.alardat.isOnOff = 0; //更新报警栏取消的报警
                        SendMeg.alardat = alm;
                        //MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, null, null);
                    }
                    collecncdata.hncdata.currentAlarmList.RemoveAt(i);
                }
            }
        }

        private static void addAlarmCurrentAlarmLst(HNCData data, AlarmData alm)
        {
            if (alm.isOnOff == 1)
            {
                if (data.currentAlarmList.Count > 0)//当前的报警列表加新加的报警
                {
                    bool find = false;
                    for (Int32 i = 0; i < data.currentAlarmList.Count; i++)
                    {
                        if (data.currentAlarmList[i].alarmNo == alm.alarmNo
                            && data.currentAlarmList[i].alarmTxt == alm.alarmTxt
                            && data.currentAlarmList[i].isOnOff == 1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        data.currentAlarmList.Add(alm);
                    }
                }
                else
                {
                    data.currentAlarmList.Add(alm);
                }
            }
            /*else  //alm.isOnOff == 0
            {
                for (Int32 i = 0; i < data.currentAlarmList.Count; i++)
                {
                    if (data.currentAlarmList[i].alarmNo == alm.alarmNo
                        && data.currentAlarmList[i].alarmTxt == alm.alarmTxt
                        && data.currentAlarmList[i].isOnOff == 1)
                    {
                        data.currentAlarmList.RemoveAt(i);
                        break;
                    }
                }
            }*/
            data.alarmList.Add(alm);
        }


        public bool CollectConstData()
        {
            bool result = false;
            result = gatherSYS.GatherConstData(hncdata.sysData);

            foreach (ChannelData chdata in hncdata.chanDataLst)
            {
                result = gatherCH.GatherConstData(chdata, hncdata.sysData.dbNo) || result;//.clientNo hxb  2017.4.18
            }

            foreach (AxisData axdata in hncdata.axlist)
            {
                result = gatherAXIS.GatherConstData(axdata, hncdata.sysData.dbNo) || result;//.clientNo hxb  2017.4.18
            }

//             bool result = true;
//             result = gatherSYS.GatherConstData(hncdata.sysData);
// 
//             foreach (ChannelData chdata in hncdata.chanDataLst)
//             {
//                 result = gatherCH.GatherConstData(chdata, hncdata.sysData.clientNo) && result;
//             }
// 
//             foreach (AxisData axdata in hncdata.axlist)
//             {
//                 result = gatherAXIS.GatherConstData(axdata, hncdata.sysData.clientNo) && result;
//             }

            return result;
        }

        public void ThreadStart()
        {
            //collectThread = new Thread(new ThreadStart(DataCollectThread));
            collectThread = new Thread(new ThreadStart(DataCollectThreadProc));// net_to_redis
            collectThread.Start(); 
        }

        public void ThreadStop()
        {
            bCollect = false;
            if (System.Threading.ThreadState.Unstarted != collectThread.ThreadState)
            {
                if (!threaFucRuningF_OK)
                {
                    threaFucRuningF_OK = true;
                    Get_Reg_threaFucEvent.Set();
                }
                collectThread.Join();
                collectThread = null;
//                 SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                 SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                 SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                 SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                 SendParm.Keywords = "CNC采集器正常退出";
//                 SendParm.EventData = m_cnc.BujianID + ":ip = " + hncdata.sysData.addr.ip;
//                 SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
            }

        }

        public System.Threading.AutoResetEvent Get_Reg_threaFucEvent = new System.Threading.AutoResetEvent(true);
        public bool threaFucRuningF_OK = false;
        static object GatherDataLockOBJ = new object();
        private int baogongold = 0;
        /*public void DataCollectThread()
        {
            int iiiii = 0;
            while (bCollect)
            {

                if (threaFucRuningF_OK)
                {
                    string macSN = "";
                    if(HNCAPI.HncApi.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, hncdata.sysData.clientNo) == 0)
                    {
                        iiiii = 0;
                        if(macSN != hncdata.sysData.macSN)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip +"：本次获取SN=" + macSN + "；上周期SN=" + hncdata.sysData.macSN;
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    else
                    {
                        iiiii++;
                        if (iiiii >= 10)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip + "：累计连续10次调用网络接口失败！";
                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    GatherData();

                    LineDevice.CNC.CNCState CNCStatei = LineDevice.CNC.CNCState.DISCON;
                    if (m_cnc != null && m_cnc.Checkcnc_state(ref CNCStatei) && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                        senddata[1] = (int)CNCStatei;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                    }
                    if (m_cnc != null && baogongold != m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCReport;
                        senddata[1] = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                        baogongold = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                    }

                    System.Threading.Thread.Sleep(50);
                }
                else
                {
                    Get_Reg_threaFucEvent.WaitOne();
                    iiiii = 0;
                }
            }
        }
        */

        public bool PingTestCNC(string ip, int timeout)
        {
            bool connect = false;
            Ping pingtest = new Ping();
            try
            {
                PingReply reply = pingtest.Send(ip, timeout);
                if (reply.Status == IPStatus.Success)
                    connect = true;
            }
            catch
            {
            }
            return connect;
        }

        // net_to_redis
        /// <summary>
        /// 采集热数据
        /// </summary>
        public void DataCollectThreadProc()
        {
            int delayCount = 0;
            while (bCollect)
            {
                //Console.WriteLine("bCollect={0}", bCollect);
                if (threaFucRuningF_OK)
                {
                    string macSN = "";
                    //if (CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_SN_NUM, ref macSN, hncdata.sysData.clientNo) == 0)
                    if (MacDataService.GetInstance().GetKeyValueString(hncdata.sysData.dbNo, "Machine", ref macSN) == 0)// 改为redis接口获取SNNUM
                    {
                        delayCount = 0;
                        bool connect = PingTestCNC(hncdata.sysData.addr.ip, 500);
                        //Console.WriteLine("IP={0}, SN= {1}, NO={2}", hncdata.sysData.addr.ip, hncdata.sysData.macSN, hncdata.sysData.dbNo);
                        if (macSN != hncdata.sysData.macSN || connect != true)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip + "：本次获取SN=" + macSN + "；上周期SN=" + hncdata.sysData.macSN;
                            //SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }
                    else
                    {
                        delayCount++;
                        if (delayCount >= 10)
                        {
                            hncdata.sysData.isConnect = false;
                            threaFucRuningF_OK = false;
                            CollectShare.threaFucEvent.Set();

                            if (m_cnc.CNCchanDataEventHandler != null)
                            {
                                int[] senddata = new int[2];
                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                senddata[1] = (int)LineDevice.CNC.CNCState.DISCON;
                                m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                            }

                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                            SendParm.Keywords = "CNC采集器关闭";
                            SendParm.EventData = hncdata.sysData.addr.ip + "：累计连续10次调用网络接口失败！";
                            //SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                        }
                    }

                    // 热数据采集
                    GatherData();

                    LineDevice.CNC.CNCState CNCStatei = LineDevice.CNC.CNCState.DISCON;
                    if (m_cnc != null && m_cnc.Checkcnc_state(ref CNCStatei) && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                        senddata[1] = (int)CNCStatei;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                    }
                    //hxb   2017.3.31
                    if (m_cnc != null && m_cnc.HCNCShareData.chanDataLst.Count>0 && baogongold != m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum && m_cnc.CNCchanDataEventHandler != null)
                    {
                        int[] senddata = new int[2];
                        senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCReport;
                        senddata[1] = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                        m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, null, null);
                        baogongold = m_cnc.HCNCShareData.chanDataLst[m_cnc.NC8_chang].partNum;
                    }

                  //  System.Threading.Thread.Sleep(50);
                    System.Threading.Thread.Sleep(200);//20180611
                }
                else
                {
                    Get_Reg_threaFucEvent.WaitOne();
                    delayCount = 0;
                }
            }
        }

        public void UpDataChAxList()
        {
            //hncdata.ConfigCH(GetClientChMask(hncdata.sysData.dbNo));
            //hncdata.ConfigAX(GetClientAxMask(hncdata.sysData.dbNo));
            hncdata.ConfigChannel(GetChannelData());
            hncdata.ConfigAxis(GetAxisData());
        }

        /// <summary>
        /// 通过redis文件夹里的数据解析通道数
        /// </summary>
        /// <returns></returns>
        public ChannelData[] GetChannelData()
        {
            ChannelData[] channelDatas = null;
            String[] keys = MacDataService.GetInstance().GetFolderKeys(hncdata.sysData.dbNo, "Channel");
            if (keys != null)
            {
                channelDatas = new ChannelData[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    channelDatas[i] = new ChannelData();
                    channelDatas[i].chNo = int.Parse(keys[i].Split(':')[1]);
                }
            }
            return channelDatas;
        }

        /// <summary>
        /// 通过redis文件夹里的数据解析轴数
        /// </summary>
        /// <returns></returns>
        public AxisData[] GetAxisData()
        {
            AxisData[] axisDatas = null;
            String[] keys = MacDataService.GetInstance().GetFolderKeys(hncdata.sysData.dbNo, "Axis");
            if (keys != null)
            {
                axisDatas = new AxisData[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    axisDatas[i] = new AxisData();
                    axisDatas[i].axisNo = int.Parse(keys[i].Split(':')[1]);
                }
            }
            return axisDatas;
        }
  

/*        /// <summary>
        /// 获取通道掩码
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        private Int32 GetClientChMask(Int16 clientNo)
        {
            Int32 mask = 0;

            Int32 ret = 0;
            Int32 ch = 0;
            Int32 chanIsExist = 0;

            for (ch = 0; ch < HNCDATADEF.SYS_CHAN_NUM; ch++)
            {
                ret = HncApi.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_IS_EXIST, ch, 0, ref chanIsExist, clientNo);
                if ((chanIsExist == 1) && (ret == 0))
                {
                    mask |= (1 << ch);
                }
            }

            return mask;
        }

        /// <summary>
        /// 获取轴号掩码
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        private Int32 GetClientAxMask(Int16 clientNo)
        {
            Int32 mask = 0;

            Int32 axisMask = 0;
            Int32 chNo = 0;
            Int32 chanNum = 0;
            Int32 ret = 0;
            ret = HncApi.HNC_SystemGetValue((int)HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, clientNo);
            if (ret != 0)
            {
                return mask;
            }

            for (chNo = 0; chNo < chanNum; chNo++)
            {
                ret = HncApi.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_AXES_MASK, chNo, 0, ref axisMask, clientNo);
                if (ret == 0)
                {
                    mask |= axisMask;
                }
            }

            return mask;
        }
*/
        public void Dispose()
        {
            Get_Reg_threaFucEvent.Dispose();
        }
    }
}