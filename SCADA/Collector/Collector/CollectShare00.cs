using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net.NetworkInformation;
using SCADA;
using HNCAPI;
using ScadaHncData;
using LineDevice;
//-------------------------------------added by zb 20151105--------------------------------------start
using HncDataInterfaces;
using HncDACore;
//-------------------------------------added by zb 20151105----------------------------------------end

namespace Collector
{
    public class CollectShare : CollectDeviceData
    {
        static HncDACore.Monitor monitor = null;      //<---------------added by zb

        private List<HNCData> ncDatas;
        private List<MES_DISPATCH> m_Taskdata;
        
        private List<CollectHNCData> gatherHNCLst = new List<CollectHNCData>();
        private List<CollectHNCData> m_gatherHNCLst = new List<CollectHNCData>();

        private List<CNC> m_cnclist;
        private Thread m_connectThread;
        private Thread m_eventThread;
        private bool m_bEventThreadRunning;

        private object connectUpdateLock = new object();

        const Int32 CONNECT_THREAD_SLEEP_TIME = 2000;
        const Int32 EVENT_THREAD_SLEEP_TIME = 10;

        
        public CollectShare(ShareData shareData, List<CNC> cnclist)
        {
            try 
            {
                monitor = new HncDACore.Monitor();
            }
            catch(Exception e)
            {
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.System_security;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.ERROR;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.ERROR).ToString();
                SendParm.Keywords = "初始化HncDACore.Monitor失败";
                SendParm.EventData = e.ToString();
                SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(SCADA.MainForm.m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
            }
            ncDatas = shareData.ncDatas;
            m_Taskdata = shareData.m_workorderlist;
            m_cnclist = cnclist;

            m_connectThread = new Thread(new ThreadStart(ConnectThreadFunc));
            m_eventThread = new Thread(new ThreadStart(EventThreadFunc));

           

        }

        /// <summary>
        /// 采集器退出
        /// </summary>
        public void CollectExit()
        {
            m_bEventThreadRunning = false;
            threaFucEvent.Set();
            m_connectThread.Abort();
            if ((System.Threading.ThreadState.Unstarted | System.Threading.ThreadState.AbortRequested )!= m_connectThread.ThreadState)
            {
                m_connectThread.Join();
                m_connectThread = null;
            }
//             LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Connect thread exit!");
            if (System.Threading.ThreadState.Unstarted != m_eventThread.ThreadState)
            {
                m_eventThread.Join();
                m_eventThread = null;
            }
            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Event thread exit!");

            foreach (CollectHNCData item in m_gatherHNCLst)
            {
                item.ThreadStop();
            }
            LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Collect Exit!");
        }

        /// <summary>
        /// 开启采集线程的线程
        /// </summary>
        private void StartConnectThread()
        {
            for (int ii = 0; ii < m_cnclist.Count; ii++)
            {
                CNC cncref = m_cnclist[ii];
                CollectHNCData m_hcncdatanade = new CollectHNCData(ref cncref, ref ncDatas);
                m_gatherHNCLst.Add(m_hcncdatanade);
            }

            for (int ii = 0; ii < m_gatherHNCLst.Count;ii++ )
            {
                m_gatherHNCLst[ii].ThreadStart();
            }
            m_bEventThreadRunning = true;
            m_connectThread.Start();
            threaFucEvent.Set();
        }

        /// <summary>
        /// 响应事件的线程
        /// </summary>
        private void StartEventThread()
        {
            m_bEventThreadRunning = true;
            m_eventThread.Start();
        }
        
        /// <summary>
        /// 定时扫描CNC列表中是否有离线的，如果有做相应处理
        /// </summary>
        public static System.Threading.AutoResetEvent threaFucEvent = new System.Threading.AutoResetEvent(true);
        private void ConnectThreadFunc()
        {
            bool ConnectNoFinish = false;
            while (m_bEventThreadRunning)
            {
                if (ConnectNoFinish)
                {
                    ConnectNoFinish = false;
                    foreach (CollectHNCData collecncdata in m_gatherHNCLst)
                    {
                        if (!collecncdata.hncdata.sysData.isConnect)
                        {
                            if (ClientPingTest(collecncdata.hncdata.sysData.addr.ip))
                            {
                                collecncdata.hncdata.sysData.dbNo = HNCAPI.HncApi.HNC_NetConnect(collecncdata.hncdata.sysData.addr.ip, collecncdata.hncdata.sysData.addr.port);
                                if (collecncdata.hncdata.sysData.dbNo >= 0 && collecncdata.hncdata.sysData.dbNo < 256)
                                {
//                                     for (int hageii = 0; hageii < 10; hageii++)
                                    {
                                        collecncdata.UpDataChAxList();
//                                         if (collecncdata.hncdata.chanDataLst.Count > 0 && collecncdata.hncdata.axlist.Count > 0)
//                                         {
//                                             break;
//                                         }
                                    }
                                    if (collecncdata.hncdata.chanDataLst.Count > 0 && collecncdata.hncdata.axlist.Count > 0)//确保采集器是初始化OK
                                    {
                                        collecncdata.CollectConstData();
                                        if (collecncdata.hncdata.sysData.dbNo >= 0 && collecncdata.hncdata.sysData.dbNo < 256
                                            && collecncdata.hncdata.sysData.macSN != null && collecncdata.hncdata.sysData.macSN.Length > 0)
                                        {
                                            collecncdata.threaFucRuningF_OK = true;
                                            collecncdata.Get_Reg_threaFucEvent.Set();
                                            collecncdata.hncdata.sysData.isConnect = true;

                                            if (collecncdata.m_cnc.CNCchanDataEventHandler != null)
                                            {
                                                int[] senddata = new int[2];
                                                senddata[0] = (int)LineDevice.CNC.CNCchanDataEventType.CNCStateChage;
                                                senddata[1] = (int)LineDevice.CNC.CNCState.IDLE;
                                                collecncdata.m_cnc.CNCchanDataEventHandler.BeginInvoke(this, senddata, new AsyncCallback(collecncdata.m_cnc.CNCchanDataEventHandlerFucFinish), "CNCStateChage Finish!");
                                            }

                                            AlarmData alm = new AlarmData();
                                            HNCAPI.HncApi.HNC_AlarmRefresh(collecncdata.hncdata.sysData.dbNo);
                                            int alarm_num = 0;
                                            HNCAPI.HncApi.HNC_AlarmGetNum((int)HNCAPI.AlarmType.ALARM_TYPE_ALL, (int)HNCAPI.AlarmLevel.ALARM_ERR, ref alarm_num, collecncdata.hncdata.sysData.dbNo);
                                            for (int i = 0; i < alarm_num; i++)
                                            {
                                                HNCAPI.HncApi.HNC_AlarmGetData((int)HNCAPI.AlarmType.ALARM_TYPE_ALL,  //获取所有类型的报警
                                                                         (int)HNCAPI.AlarmLevel.ALARM_ERR, //仅获取error
                                                                         i,
                                                                         ref alm.alarmNo,     //获取此报警的唯一ID，可用于报警识别
                                                                         ref alm.alarmTxt,             //报警文本
                                                                         collecncdata.hncdata.sysData.dbNo);
                                                alm.isOnOff = 1;
                                                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                                                {
                                                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                                                     SendMeg.alardat = new ScadaHncData.AlarmData();
                                                    SendMeg.NeedFindTeX = false;
                                                    SendMeg.BujianID = collecncdata.m_cnc.BujianID;
                                                    SendMeg.alardat.isOnOff = 1;
                                                    SendMeg.alardat = alm;
                                                    SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, new AsyncCallback(SCADA.MainForm.m_CheckHander.EquipMentAlarTexChangeFucFinish), "EquipMentSateShangeFucFinish!");
                                                }

                                                UpdateCurrentAlarmLst(collecncdata.hncdata, alm);
                                            }
                                            SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                                            SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                                            SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                                            SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                                            SendParm.Keywords = "CNC采集器开启";
                                            SendParm.EventData = collecncdata.m_cnc.BujianID + ":ip = " + collecncdata.hncdata.sysData.addr.ip
                                                + "  链接号 = " + collecncdata.hncdata.sysData.dbNo.ToString()
                                                               + " sn = " + collecncdata.hncdata.sysData.macSN;
                                            SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(SCADA.MainForm.m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                                        }
                                    }
                                    else
                                    {
//                                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                                         SendParm.Keywords = "collecncdata.hncdata.chanDataLst.Count = 0";
//                                         SendParm.EventData = collecncdata.hncdata.sysData.addr.ip;
//                                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
                                    }
                                }
                            }
                        }
                        if (!collecncdata.hncdata.sysData.isConnect)
                        {
                            ConnectNoFinish = true;
                        }
                    }
                }
                else
                {
                    threaFucEvent.WaitOne();
                    ConnectNoFinish = true;
                }

                //---------------------------------added by zb 20151105--------------------------start
                if (ncDatas.Count > 0)
                    monitor.ProcessConnDatPosi(ncDatas);
                //---------------------------------added by zb 20151105--------------------------end

                //                 Thread.Sleep(CONNECT_THREAD_SLEEP_TIME);
            }
        }

        /// <summary>
        /// ping IP测试
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private Boolean ClientPingTest(String ip)
        {
            try 
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 1000);
                if (pingReply.Status == IPStatus.Success)
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


        private void EventThreadFunc()
        {
            int ret = 0;
            short clientNo = 0;
            short ch = 0;
//             CollectHNCData collecncdata;

            Byte[] newbuf = new Byte[128];
            Byte[] buf = new Byte[128];
            HNCAPI.SEventElement ev = new HNCAPI.SEventElement();
            SAlarmEvent almEV = new SAlarmEvent();
            HncDataInterfaces.SEventElement evTemp = new HncDataInterfaces.SEventElement();

            while (m_bEventThreadRunning)
            {
                //读事件
                //1.获取是否有新的client连接
                //2.获取告警状态、内容
                //3.获取加工信息
                ret = HNCAPI.HncApi.HNC_EventGetSysEv(ref ev);
                if (ret != 0)
                {
                    continue;
                }
                GetClientCh(ev, ref clientNo, ref ch);
                string macSN = "";
                ret = HNCAPI.HncApi.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
                String ip = String.Empty;

                //按事件类型来处理
                //1.网络事件 ncEvtConnect,ncEvtDisConnect,ncEvtAlarmChg
                //2.用户自定义事件 ncEvtChFin0， ncEvtChFin1，ncEvtChFin2，ncEvtChFin3
                switch (ev.code)
                {
                    case EVENTDEF.ncEvtConnect: //新增用户
//                         find = VerifyClient(clientNo);//校验是否存在该机床，如果为新则增加，否则更新机床clientNo  
//                         if (find)
//                         {
//                             StartCollectBase(clientNo);
//                             UpdateHNCDataStat(clientNo);
//                         }
// 
//                         collecncdata = m_gatherHNCLst.Find(
//                             delegate(CollectHNCData temp)
//                             {
//                                 return (temp.hncdata.sysData.addr.ip == ip && temp.hncdata.sysData.addr.port == port);
//                             }
//                             );
// 
//                         if (collecncdata == null)
//                         {
//                             return;
//                         }
//                         collecncdata.hncdata.sysData.clientNo = clientNo;
//                         collecncdata.UpDataChAxList();
//                         collecncdata.CollectConstData();
//                         collecncdata.threaFucRuningF_OK = true;
//                         collecncdata.Get_Reg_threaFucEvent.Set();
//                         collecncdata.hncdata.sysData.isConnect = true;
//                         LogApi.WriteLogInfo(MainForm.logHandle, (byte)ENUM_LOG_LEVEL.LEVEL_WARN, "3");

                        break;

                    case EVENTDEF.ncEvtDisConnect://用户下线
//                             collecncdata = m_gatherHNCLst.Find(
//                             delegate(CollectHNCData temp)
//                             {
//                                 return (temp.hncdata.sysData.clientNo == clientNo);
//                             }
//                             );
// 
//                             if (collecncdata == null)
//                             {
//                                 return;
//                             }
//                             if (collecncdata.hncdata.sysData.clientNo == clientNo)
//                             {
//                                 collecncdata.hncdata.sysData.isConnect = false;
//                                 collecncdata.threaFucRuningF_OK = false;
//                                 collecncdata.hncdata.sysData.clientNo = -1;
//                                 threaFucEvent.Set();
//                                 SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                                 SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                                 SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                                 SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                                 SendParm.Keywords = "CNC采集器关闭";
//                                 SendParm.EventData = collecncdata.hncdata.sysData.addr.ip +  "收到用户下线通知事件";
//                                 SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                             }
                        break;

                    case EVENTDEF.ncEvtAlarmChg://告警上报、消除
                        almEV = CGlbFunc.ByteToStructure<SAlarmEvent>(ev.buf, 4);//从4开始偏移
                        GatherAlarmData(almEV, clientNo);
                        break;

                    case EVENTDEF.ncEvtPrgStart:
                        UpdateLastProgStartTime(clientNo);
                        break;

                    case EVENTDEF.ncEvtChFin0://用户加工完成时间 
                    case EVENTDEF.ncEvtChFin1:
                    case EVENTDEF.ncEvtChFin2:
                    case EVENTDEF.ncEvtChFin3:
//                         collecncdata = m_gatherHNCLst.Find(
//                                         delegate(CollectHNCData temp)
//                                         {
//                                             return (temp.hncdata.sysData.clientNo == clientNo);
//                                         }
//                                         );
//                         if (collecncdata != null)
//                         {
//                             SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                             SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                             SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                             SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                             SendParm.Keywords = "CNC报工触发";
//                             SendParm.EventData = collecncdata.m_cnc.BujianID;
//                             SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                         }
// 
//                         GatherMachineData(ch, clientNo, ev.buf);
                        break;

                    default:
                        break;
                }
                //---------------------------------added by zb 20151105--------------------------start
                //HncDataInterfaces.SEventElement evTemp = new HncDataInterfaces.SEventElement();
                evTemp.buf = ev.buf;
                evTemp.code = ev.code;
                evTemp.src = ev.src;

                if (ncDatas.Count > 0)
                    monitor.ReprocessEvent(ncDatas, evTemp);
                //---------------------------------added by zb 20151105----------------------------end
            }

            Thread.Sleep(EVENT_THREAD_SLEEP_TIME);
        }

        private void UpdateLastProgStartTime(Int16 clientNo)
        {
            HNCData result = ncDatas.Find(
                   delegate(HNCData temp)
                   {
                       return temp.sysData.dbNo == clientNo;
                   }
                   );

            if (result == null)
            {
                return;
            }

            result.sysData.lastProgStartTime = DateTime.Now;
        }

        public void StartCollectBase(Int16 clientNo)
        {
            string macSN = "";
            int ret = 0;
            //开启一个线程隔N长事件收集一次
            HNCData result = ncDatas.Find(
                     delegate(HNCData temp)
                     {
                         return temp.sysData.dbNo == clientNo;
                     }
                     );

            if (result == null)
            {
                return;                
            }

            ret = HNCAPI.HncApi.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
            if (ret != 0)
            {
                return ;
            }
            CollectHNCData colresult = gatherHNCLst.Find(
                    delegate(CollectHNCData temp)
                    {
                        return String.Equals(temp.hncdata.sysData.macSN, macSN, StringComparison.Ordinal);
                    }
                    );

            if (colresult == null)//没有重复机床，第一次加入
            {
//                 CollectHNCData gatherHdata = new CollectHNCData(result);
//                 gatherHNCLst.Add(gatherHdata);
//                 gatherHdata.ThreadStart();
            }
            else //以前有过同SN的机床
            { 
                if (colresult.collectThread == null)
                {
//                     colresult.bCollect = true;
//                     colresult.ThreadStart();
                }
            }
        }

        public void StopCollectBase(short clientNo)
        {
            String sLogInfo = String.Empty;

            CollectHNCData result = gatherHNCLst.Find(
                  delegate(CollectHNCData temp)
                  {
                      return temp.hncdata.sysData.dbNo == clientNo;
                  });
            if (result == null)
            {
                return;
            }
            else
            {
//                 sLogInfo = "Client disconnect but no ncdata matched! clientNo = " + clientNo.ToString();
//                 LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
            }

            CNC cncItem = m_cnclist.Find(
                delegate(CNC temp)
                {
                    return (temp.ip == result.hncdata.sysData.addr.ip
                            && temp.port == result.hncdata.sysData.addr.port);
                });
            if (cncItem != null)
            {
                //cncItem.HCNCShareData = null;
//                 cncItem.isConnect = false;
            }

            result.bCollect = false;
            result.collectThread.Join();
            result.hncdata.sysData.dbNo = -1;

//             sLogInfo = "Client disconnect! clientNo = " + clientNo.ToString()
//                                + "ip = " + cncItem.ip + "port = " + cncItem.port.ToString();
//             LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, sLogInfo);
        }

        /// <summary>
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
                ret = HNCAPI.HncApi.HNC_ChannelGetValue((int)HNCAPI.HncChannel.HNC_CHAN_IS_EXIST, ch, 0, ref chanIsExist, clientNo);
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
            ret = HNCAPI.HncApi.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, clientNo);
            if (ret != 0)
            {
                return mask;
            }

            for (chNo = 0; chNo < chanNum; chNo++)
            {
                ret = HNCAPI.HncApi.HNC_ChannelGetValue((int)HNCAPI.HncChannel.HNC_CHAN_AXES_MASK, chNo, 0, ref axisMask, clientNo);
                if (ret == 0)
                {
                    mask |= axisMask;
                }
            }

            return mask;
        }

//         public void AddClient(Int16 clientNo)
//         {
//             Int32 chMask = GetClientChMask(clientNo);
//             Int32 axMask = GetClientAxMask(clientNo);
//             HNCData hncdata = new HNCData(clientNo, chMask, axMask);
// 
//             String ip = String.Empty;
//             UInt16 port = 0;
//             Int32 ret = HncApi.HNC_NetGetIpaddr(ref ip, ref port, clientNo);
//             if (ret == 0)
//             {
//                 hncdata.sysData.addr.ip = ip;
//                 hncdata.sysData.addr.port = port;
//             }
// 
//             ncDatas.Add(hncdata);
//         }              

        void GetClientCh(HNCAPI.SEventElement ev, ref short clientNo, ref  short ch)
        {
            short[] info = new short[2];
            Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
            clientNo = info[0];
            ch = info[1];        
        }

        /// <summary>
        /// 验证连接号为clientNo的cnc是否存在
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public bool VerifyClient(short clientNo)
        {
            int ret = 0;
            string macSN = "";
            bool flag = false;
            ret = HNCAPI.HncApi.HNC_SystemGetValue((int)HNCAPI.HncSystem.HNC_SYS_SN_NUM, ref macSN, clientNo);
            if (ret != 0)
            {
                return false;
            }
               
            HNCData result = ncDatas.Find(
                      delegate(HNCData temp)
                      {                            
                          return String.Equals(temp.sysData.macSN, macSN, StringComparison.Ordinal);
                      }
                     );

            if (result == null) //没找到
            {
                flag = true;
            }
            else //找到同SN的机床，以最新clientNo为准
            {
                flag = true;
                if (result.sysData.dbNo != clientNo)
                {
                    result.sysData.dbNo = clientNo;
                }
            }

            return flag;
        }

        public void GatherAlarmData(SAlarmEvent almEV,short clientNo)
        {
            AlarmData alm = new AlarmData();
            alm.time = DateTime.Now;
            alm.isOnOff = almEV.begin;
            alm.alarmNo = almEV.alarmNo;
            alm.alarmTxt = System.Text.Encoding.Default.GetString(almEV.alarmText).Trim('\0');

            LineDevice.CNC result = MainForm.cnclist.Find(
                    delegate(LineDevice.CNC temp)
                    {
                        return temp.HCNCShareData.sysData.dbNo == clientNo;
                    }
                   );

            if (result != null)
            {
                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                {
                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                     SendMeg.alardat = new ScadaHncData.AlarmData();
                    SendMeg.NeedFindTeX = false;
                    SendMeg.BujianID = result.BujianID;
                    SendMeg.alardat = alm;
                    SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, new AsyncCallback(SCADA.MainForm.m_CheckHander.EquipMentAlarTexChangeFucFinish), "EquipMentSateShangeFucFinish!");
                }

                if (result.HCNCShareData.alarmList.Count == result.HCNCShareData.AlmLstLen)
                {
                    result.HCNCShareData.alarmList.RemoveRange(0, result.HCNCShareData.AlmLstLen / 2);
                }
                UpdateCurrentAlarmLst(result.HCNCShareData, alm);
            }
            else if (PLCDataShare.m_plclist != null && PLCDataShare.m_plclist.Count > 0 &&
                PLCDataShare.m_plclist[0].m_hncPLCCollector != null && clientNo == Collector.CollectHNCPLC.m_clientNo)//HNC8 PLC报警
            {
                if (MainForm.m_CheckHander != null && MainForm.m_CheckHander.AlarmSendDataEvenHandle != null)
                {
                    EquipmentCheck.AlarmSendData SendMeg = new EquipmentCheck.AlarmSendData();
//                     SendMeg.alardat = new ScadaHncData.AlarmData();
                    SendMeg.NeedFindTeX = false;
                    SendMeg.BujianID = PLCDataShare.m_plclist[0].m_hncPLCCollector.EQUIP_CODE;
                    SendMeg.alardat = alm;
                    SCADA.MainForm.m_CheckHander.AlarmSendDataEvenHandle.BeginInvoke(null, SendMeg, new AsyncCallback(SCADA.MainForm.m_CheckHander.EquipMentAlarTexChangeFucFinish), "EquipMentSateShangeFucFinish!");
                }
            }
        }

        private void UpdateCurrentAlarmLst(HNCData data, AlarmData alm)
        {

            if (alm.isOnOff == 1)
            {
                if (data.currentAlarmList.Count > 0)
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
            else
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
            }
            data.alarmList.Add(alm);
        }




        /// <summary>
        /// 报工函数
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="clientNo"></param>
        /// <param name="buf"></param>
//         public void GatherMachineData(int ch,short clientNo,Byte[] buf)
//         {
//             CollectHNCData result = m_gatherHNCLst.Find(
//                             delegate(CollectHNCData temp)
//                             {
//                                 return (temp.hncdata.sysData.clientNo == clientNo);
//                             }
//                             );
// 
// 
//             if (result == null)
//             {
//                 return;
//             }
//             if (result.m_cnc.mesDispatch != null && result.m_cnc.mesDispatch.PLAN_SENDED == 2)
//             {
//                 PROD_REPORT machinedata = new PROD_REPORT();
//                 machinedata.END_TIME = System.DateTime.Now;
//                 machinedata.START_TIME = result.hncdata.sysData.lastProgStartTime;
//                 machinedata.EQUIP_CODE = result.m_cnc.BujianID;
//                 machinedata.ASSIGN_CODE = result.m_cnc.mesDispatch.ORDER_CODE;
//                 if (result.m_cnc.mesDispatch.FLAG.ToString().Length > 0)
//                 {
//                     machinedata.FLAG = sbyte.Parse((result.m_cnc.mesDispatch.FLAG.ToString()));
//                 }
//                 machinedata.OP_CODE = result.m_cnc.mesDispatch.OP_CODE;//工序编码
//                 machinedata.MATERIAL_CODE = result.m_cnc.mesDispatch.MATERIAL_CODE;//物料编码
//                 machinedata.SN = result.m_cnc.mesDispatch.SN;
//                 machinedata.ASSIGN_CODE = result.m_cnc.mesDispatch.DISPATCH_CODE;//派工单号
// 
//                 machinedata.MARK_DATE = result.m_cnc.mesDispatch.MARK_DATE;
//                 machinedata.QTY = 1;//报工数
// 
// 
//                 if (result.m_cnc.OneInOneOut)
//                 {
//                     if (result.m_cnc.RFIDreportData.ChanPingXuLieHao[0] != null &&
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[0].Length != 0)
//                     {
//                         machinedata.PRODUCT_SN = result.m_cnc.RFIDreportData.ChanPingXuLieHao[0];
//                         result.hncdata.reportList.Add(machinedata);
// //                         result.m_cnc.reportCount++;
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[0] = null;//清空
// 
//                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                         SendParm.Keywords = "CNC报工";
//                         SendParm.EventData = result.m_cnc.BujianID + "一出一：" + machinedata.PRODUCT_SN;
//                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                     }
//                     else if (result.m_cnc.RFIDreportData.ChanPingXuLieHao[1] != null &&
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[1].Length != 0)
//                     {
//                         machinedata.PRODUCT_SN = result.m_cnc.RFIDreportData.ChanPingXuLieHao[1];
//                         result.hncdata.reportList.Add(machinedata);
// //                         result.m_cnc.reportCount++;
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[1] = null;//清空
//                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                         SendParm.Keywords = "CNC报工";
//                         SendParm.EventData = result.m_cnc.BujianID + "一出一：" + machinedata.PRODUCT_SN;
//                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                     }
//                 }
//                 else
//                 {
//                     if (result.m_cnc.RFIDreportData.ChanPingXuLieHao[0] != null &&
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[0].Length != 0)
//                     {
//                         machinedata.PRODUCT_SN = result.m_cnc.RFIDreportData.ChanPingXuLieHao[0];
//                         result.hncdata.reportList.Add(machinedata);
// //                         result.m_cnc.reportCount++;
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[0] = null;//清空
//                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                         SendParm.Keywords = "CNC报工";
//                         SendParm.EventData = result.m_cnc.BujianID + "一出二：" + machinedata.PRODUCT_SN;
//                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                     }
//                     if (result.m_cnc.RFIDreportData.ChanPingXuLieHao[1] != null &&
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[1].Length != 0)
//                     {
//                         PROD_REPORT machinedata1 = new PROD_REPORT();
//                         machinedata1.ASSIGN_CODE = machinedata.ASSIGN_CODE;
//                         machinedata1.END_TIME = machinedata.END_TIME;
//                         machinedata1.EQUIP_CODE = machinedata.EQUIP_CODE;
//                         machinedata1.FLAG = machinedata.FLAG;
//                         machinedata1.MARK_DATE = machinedata.MARK_DATE;
//                         machinedata1.MARK_TIME = machinedata.MARK_TIME;
//                         machinedata1.MATERIAL_CODE = machinedata.MATERIAL_CODE;
//                         machinedata1.OP_CODE = machinedata.OP_CODE;
//                         machinedata1.QTY = machinedata.QTY;
//                         machinedata1.SN = machinedata.SN;
//                         machinedata1.START_TIME = machinedata.START_TIME;
//                         machinedata1.PRODUCT_SN = result.m_cnc.RFIDreportData.ChanPingXuLieHao[1];
//                         result.hncdata.reportList.Add(machinedata1);
// //                         result.m_cnc.reportCount++;
//                         result.m_cnc.RFIDreportData.ChanPingXuLieHao[1] = null;//清空
//                         SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
//                         SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
//                         SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.消息;
//                         SendParm.EventID = ((int)SCADA.LogData.Node2Level.消息).ToString();
//                         SendParm.Keywords = "CNC报工";
//                         SendParm.EventData = result.m_cnc.BujianID + "一出二：" + machinedata.PRODUCT_SN;
//                         SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);
//                     }
//                 }
// 
// 
//                 if (result.hncdata.reportList.Count == result.hncdata.MachLstLen)
//                 {
//                     result.hncdata.reportList.RemoveRange(0, result.hncdata.MachLstLen / 2);
//                 }
//             }
//         }

        public void StartCollection()
        {
            StartConnectThread();
            StartEventThread();
//             LogApi.WriteLogInfo(MainForm.logHandle, (Byte)ENUM_LOG_LEVEL.LEVEL_WARN, "Collect Start!");
        }
    }
}