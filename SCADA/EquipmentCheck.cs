using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.NewApp;

namespace SCADA
{
    public class EquipmentCheck
    {
        public String[] CheckdataGridView_title = { "序号", "设备编码", "名称", "状态", "报警内容" };
        public String[] ENCheckdataGridView_title = { "SN", "MachineCode", "Name", "Status", "AlarmContent" };
        public enum CheckdataGridView_titleArr_Index
        {
            序号 = 0,
            设备编码,
            名称,
            状态,
            报警内容
        }
        public bool CheckdataGridView_DB_ChangeFlg = false;
        private System.Data.DataTable CheckdataGridView_DB = new System.Data.DataTable();
        public class AlarmSendData
        {
            public AlarmSendData()
            {
                this.NeedFindTeX = false;
                this.BujianID = "";
                this.alardat = new ScadaHncData.AlarmData();
            }
            public bool NeedFindTeX;
            public String BujianID;
            public ScadaHncData.AlarmData alardat;
        }
        private Dictionary<int, string> PLCAlarmTab = new Dictionary<int, string>();//PLC报警内容和报警号的对照字典

        public System.EventHandler<AlarmSendData> AlarmSendDataEvenHandle;
        public System.EventHandler<ScadaHncData.EQUIP_STATE> StateChageEvenHandle;
        public EquipmentCheck()
        {
            InitPLCAlarmNoTb();
            lock (CheckdataGridView_DBCLock)
            {
                for (int ii = 0; ii < CheckdataGridView_title.Length; ii++)
                {
                    CheckdataGridView_DB.Columns.Add(CheckdataGridView_title[ii]);
                }

                //LoadEquipLanguage(ChangeLanguage.GetDefaultLanguage());
                String[] rowstr = new String[CheckdataGridView_title.Length];


                string get_str = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);//SUM
                int CNCSUM = int.Parse(get_str);
                for (int ii = 0; ii < CNCSUM; ii++)//CNC
                {
                    rowstr[(int)CheckdataGridView_titleArr_Index.序号] = CheckdataGridView_DB.Rows.Count.ToString();
                    rowstr[(int)CheckdataGridView_titleArr_Index.设备编码] = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.EQUIP_CODE]);
                    if (rowstr[(int)CheckdataGridView_titleArr_Index.设备编码].Length - 0 < 0)  //hxb  2017.3.30
                        continue;
                    rowstr[(int)CheckdataGridView_titleArr_Index.名称] = ChangeLanguage.GetString("EquipmentMachine") + rowstr[(int)CheckdataGridView_titleArr_Index.设备编码].Substring(rowstr[(int)CheckdataGridView_titleArr_Index.设备编码].Length - 3, 3);
                    rowstr[(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentOffline");
                    CheckdataGridView_DB.Rows.Add(rowstr);
                }

                //get_str = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                //for (int ii = 0; ii < int.Parse(get_str); ii++)
                //{
                  
                //    rowstr[(int)CheckdataGridView_titleArr_Index.序号] = CheckdataGridView_DB.Rows.Count.ToString();
                //    rowstr[(int)CheckdataGridView_titleArr_Index.名称] = "PLC";
                //    rowstr[(int)CheckdataGridView_titleArr_Index.设备编码] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.EQUIP_CODE]);
                //    CheckdataGridView_DB.Rows.Add(rowstr);

                //    String PLCSystem = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]);
                //    String[] Device;
                //    if (PLCSystem == m_xmlDociment.PLC_System[0])
                //    {
                //        Device = new String[m_xmlDociment.Default_MITSUBISHI_Device1.Length + m_xmlDociment.Default_MITSUBISHI_Device2.Length];
                //        for (int qq = 0; qq < m_xmlDociment.Default_MITSUBISHI_Device1.Length; qq++)
                //        {
                //            Device[qq] = m_xmlDociment.Default_MITSUBISHI_Device1[qq];
                //        }
                //        for (int qq = 0; qq < m_xmlDociment.Default_MITSUBISHI_Device2.Length; qq++)
                //        {
                //            Device[m_xmlDociment.Default_MITSUBISHI_Device1.Length + qq] = m_xmlDociment.Default_MITSUBISHI_Device2[qq];
                //        }
                //    }
                //    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                //    {
                //        Device = new String[m_xmlDociment.Default_HNC8_Device1.Length + m_xmlDociment.Default_HNC8_Device2.Length];
                //        for (int qq = 0; qq < m_xmlDociment.Default_HNC8_Device1.Length; qq++)
                //        {
                //            Device[qq] = m_xmlDociment.Default_HNC8_Device1[qq];
                //        }
                //        for (int qq = 0; qq < m_xmlDociment.Default_HNC8_Device2.Length; qq++)
                //        {
                //            Device[m_xmlDociment.Default_HNC8_Device1.Length + qq] = m_xmlDociment.Default_HNC8_Device2[qq];
                //        }
                //    }
                //    else
                //    {
                //        Device = new String[0];
                //    }
                //    for (int jj = 0; jj < Device.Length; jj++)
                //    {
                //        string pathstr1 = m_xmlDociment.PathRoot_PLC_Item + ii.ToString();//Root/PLC/Itemii
                //        string pathstr2 = pathstr1 + "/" + Device[jj];//"";
                //        if (!MainForm.m_xml.CheckNodeExist(pathstr2))
                //        {
                //            continue;
                //        }

                //        Int32 Count = Int32.Parse(MainForm.m_xml.m_Read(pathstr2, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
                //        for (int kk = 0; kk < Count; kk++)
                //        {
                //            if (MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.ACTION_ID]) == "PLCALARM")//PLC自定义的报警
                //            {
                //                rowstr[(int)CheckdataGridView_titleArr_Index.序号] = CheckdataGridView_DB.Rows.Count.ToString();
                //                rowstr[(int)CheckdataGridView_titleArr_Index.名称] = MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.name]);
                //                rowstr[(int)CheckdataGridView_titleArr_Index.设备编码] = MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.EQUIP_CODE]);
                //               CheckdataGridView_DB.Rows.Add(rowstr);
                //            }
                //        }
                //    }
                //}
                for (int ii = CNCSUM; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentOffline");

                    String type = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称].ToString();
                    string StateLang = "";
                    //string lang = ChangeLanguage.GetDefaultLanguage();
                    if (type == "PLC")
                    {
                        String PLCSystem = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]);
                        CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = StateLang;
                    }
                }
            }

            StateChageEvenHandle = new EventHandler<ScadaHncData.EQUIP_STATE>(this.EquipMentSateShangeFuc);
            AlarmSendDataEvenHandle = new EventHandler<AlarmSendData>(this.EquipMentAlarTexChangeFuc);
            CheckdataGridView_DB_ChangeFlg = true;
        }

        public void LoadEquipLanguage(string lang)
        {
            int jj =0 ;
              
            if (lang == "Chinese")
            {
                for (int ii = 0; ii < CheckdataGridView_title.Length; ii++)
                {
                    CheckdataGridView_DB.Columns[ii].ColumnName = CheckdataGridView_title[ii];
                }
                for (int ii = 0; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    string PreName = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称].ToString();
                    string CurName = PreName.Replace("Machine", "机台");
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称] = CurName;
                    string StateLang = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态].ToString();
                    //如果是西门子的PLC，那么只有在线和离线两个状态；
                    String type = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称].ToString();
                    if (type == "PLC" )
                    {
                        String PLCSystem = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, jj, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]);
                        jj++;
                    }
                    else
                    {
                        if (StateLang == "Offline")
                            StateLang = "离线";
                        else if (StateLang == "Free")
                            StateLang = "空闲";
                        else if (StateLang == "Working")
                            StateLang = "运行";
                        else if (StateLang == "FeedHold")
                            StateLang = "保持进给";
                        else if (StateLang == "EmergencyStop")
                            StateLang = "急停";
                    }
                  
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = StateLang;
                }
            }
            else if (lang == "English")
            {
                for (int ii = 0; ii < ENCheckdataGridView_title.Length; ii++)
                {
                    CheckdataGridView_DB.Columns[ii].ColumnName = ENCheckdataGridView_title[ii];
                }
                for (int ii = 0; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    string PreName = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称].ToString();
                    string CurName = PreName.Replace("机台", "Machine");
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称] = CurName;
                    string StateLang = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态].ToString();
                    String type = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.名称].ToString();
                    if (type == "PLC")
                    {
                        String PLCSystem = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, jj, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]);
                  
                        jj++;
                    }
                    else
                    {
                        if (StateLang == "离线")
                            StateLang = "Offline";
                        else if (StateLang == "空闲")
                            StateLang = "Free";
                        else if (StateLang == "运行")
                            StateLang = "Working";
                        else if (StateLang == "保持进给")
                            StateLang = "FeedHold";
                        else if (StateLang == "急停")
                            StateLang = "EmergencyStop";
                    }
                    
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = StateLang;
                }
            }
            CheckdataGridView_DB_ChangeFlg = true;
        }

        private void InitPLCAlarmNoTb()
        {
//             if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]) == m_xmlDociment.PLC_System[0])//三菱
            {
                PLCAlarmTab.Clear();
                int Sum = 0;
                if (int.TryParse(LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, -1, m_xmlDociment.Default_Attributes_str1[0]), out Sum))
                {
                    for (int ii = 0; ii < Sum; ii++)
                    {
                        int no = 0;
                        if (int.TryParse(LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, ii, m_xmlDociment.Default_Attributes_PLCAlarmTb[0]), out no)
                            && !PLCAlarmTab.ContainsKey(no))
                        {
                            PLCAlarmTab.Add(no, LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, ii, m_xmlDociment.Default_Attributes_PLCAlarmTb[1]));
                        }
                    }
                }
            }
        }

        object CheckdataGridView_DBCLock = new object();
        private void EquipMentSateShangeFuc(object obj, ScadaHncData.EQUIP_STATE m_State)
        {
            /*if (LineMainForm.SendEQUIP_STATEHandler != null)//更改PLC设备状态数据
            {
                SCADA.MainForm.SendEQUIP_STATEHandler.BeginInvoke(this, m_State, null, null);
            }*/
            lock(CheckdataGridView_DBCLock)
            {
                int ii = 0;
                for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    if ((CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.设备编码]).ToString() == m_State.EQUIP_CODE)
                    {
                        switch ((int)m_State.STATE_VALUE)// FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                        {
                            case -1:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentOffline");
                                break;
                            case 0:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentFree");
                                break;
                            case 1:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentWorking");
                                break;
                            case 2:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentFeedHold");
                                break;
                            case 3:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] = ChangeLanguage.GetString("EquipmentEmergencyStop");
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
                if (ii == LineMainForm.cnclist.Count)//后续的PLC相关的状态跟踪着PLC状态变化
                {
                    for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                    {
                        CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.状态] =
                            CheckdataGridView_DB.Rows[LineMainForm.cnclist.Count][(int)CheckdataGridView_titleArr_Index.状态];
                    }
                }
            }
            CheckdataGridView_DB_ChangeFlg = true;
        }

        private void EquipMentAlarTexChangeFuc(object obj, AlarmSendData Meg)
        {
            if (Meg.NeedFindTeX)//在对照表中查找报警内容
            {
                if (!PLCAlarmTab.ContainsKey(Meg.alardat.alarmNo))
                {
                    return;
                }
                Meg.alardat.alarmTxt = PLCAlarmTab[Meg.alardat.alarmNo];
            }
            lock (CheckdataGridView_DBCLock)
            {
                int ii = 0;
                for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    if (Meg.BujianID == CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.设备编码].ToString())
                    {
                        if (!Meg.NeedFindTeX)
                        {
                            String str = /*Meg.alardat.alarmNo.ToString() + ":" +*/ Meg.alardat.alarmTxt + "；  ";
                            if (CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容].ToString().Contains(str))
                            {
                                if (Meg.alardat.isOnOff != 1)
                                {
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容] =
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容].ToString().Replace(str, "");
                                    CheckdataGridView_DB_ChangeFlg = true;
                                }
                            }
                            else
                            {
                                if (Meg.alardat.isOnOff == 1)
                                {
                                    String str1 = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容].ToString();
                                    str1 += str;
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容] = str1;
                                    CheckdataGridView_DB_ChangeFlg = true;
                                }
                            }
                        }
                        else
                        {
                            CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.报警内容] = Meg.alardat.alarmTxt;
                            CheckdataGridView_DB_ChangeFlg = true;
                        }
                        break;
                    }
                }
            }
        }

        public void EquipMentSateShangeFucFinish(IAsyncResult result)
        {
//             EventHandler<AlarmSendData> Hanlder = (EventHandler<AlarmSendData>)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
//             Hanlder.EndInvoke(result);
//             Console.Write(result);
        }
        public void EquipMentAlarTexChangeFucFinish(IAsyncResult result)
        {
//             EventHandler<ScadaHncData.EQUIP_STATE> Hanlder = (EventHandler<ScadaHncData.EQUIP_STATE>)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
//             Hanlder.EndInvoke(result);
//             Console.Write(result);
        }

        public System.Data.DataTable GetCheckdataGridView_DB()
        {
            CheckdataGridView_DB_ChangeFlg = false;
            if (CheckdataGridView_DB != null)
            {
                return this.CheckdataGridView_DB.Copy();
            }
            else
            {
                return null;
            }
        }
    }
}
