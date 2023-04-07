using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNC_MacDataService;
using ScadaHncData;
using SCADA;


namespace Collector
{
    public class CollectAxisData : CollectDeviceAXData
    {
        //private Double ConverPosUnitToMM(Int32 pos, AxisData axData)
        //{
        //    Int32 ret = 0;
        //    Int32 unit = 100000;
        //    Int32 metric = 0;
        //    Int32 axistype = 1;
        //    Double mmPos = 0;
        //    const Double METRIC_COEFF = 25.4;
            
        //    if (m_data == null || axData == null)
        //    {
        //        mmPos = (Double)pos / unit;
        //        return mmPos;
        //    }

        //    axistype = axData.axisType;
        //    metric = m_data.sysData.metric;

        //    if (axistype == 1)//直线轴
        //    {
        //        unit = m_data.sysData.moveUnit;

        //        if (0 == metric) // 英制
        //        {
        //            mmPos = (Double)pos / unit / METRIC_COEFF;
        //        }
        //        else
        //        {
        //            mmPos = (Double)pos / unit;
        //        }
        //    }
        //    else
        //    {
        //        unit = m_data.sysData.turnUnit;
        //        mmPos = (Double)pos / unit;
        //    }

        //    return mmPos;
        //}

        //static bool getFollow(int axisNo, int data, short clientNo, ref double result)
        //{
        //    int ret = 0;
        //    int dist = 0;
        //    int pulse = 0;
        //    ret = MacDataService.GetInstance().HNC_ParamanGetI32(PARAMAN_FILE_AXIS, axisNo, PAR_AX_PM_MUNIT, ref dist,clientNo);
        //    if (ret != 0)
        //    {
        //        return false;
        //    }
            
        //    ret = MacDataService.GetInstance().HNC_ParamanGetI32(PARAMAN_FILE_AXIS, axisNo, PAR_AX_PM_PULSE, ref pulse, clientNo);
        //    if (ret != 0)
        //    {
        //        return false;
        //    }
        //    if (pulse == 0)
        //    {
        //        return false;
        //    }
        //    result = data * dist / pulse / 1000;
        //    return true;
        //}

        class AxisNameCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {               
                //int ret = 0;
                //string axisName = "";
               // ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, axdata.axisNo, ref axisName, clientNo);
                int ret = 0;
                string axisName = "";
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, key, "HNC_AXIS_NAME", ref axisName);
                if (ret == 0)
                {
                    axdata.axisName = axisName;
                }
                return (ret == 0 ? true : false);
            }
        }

        class AxisTypeCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                int ret = 0;
                int axisType=0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueInt(dbNo, key, "HNC_AXIS_TYPE", ref axisType);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_TYPE, axdata.axisNo, ref axisType, dbNo);
                if (ret == 0)
                {
                    axdata.axisType = axisType;
                }
                return (ret == 0 ? true : false);
            }
        }

  /*      class AxisDistCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                const Int32 PAR_AX_PM_MUNIT = 4;// 坐标轴参数
                
                ret = MacDataService.GetInstance().HNC_ParamanGetI32((Int32)HNCDATADEF.PARAMAN_FILE_AXIS, axdata.axisNo, PAR_AX_PM_MUNIT, ref axdata.dist, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                return (ret == 0 ? true : false);
            }
        }

        class AxisPulseCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, short clientNo)
            {
                Int32 ret = 0;
                const Int32 PAR_AX_PM_PULSE = 5;// 坐标轴参数

                ret = MacDataService.GetInstance().HNC_ParamanGetI32((Int32)HNCDATADEF.PARAMAN_FILE_AXIS, axdata.axisNo, PAR_AX_PM_PULSE, ref axdata.pulse, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                return (ret == 0 ? true : false);
            }
        }
*/
        class ActPosCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                Int32 ret = 0;
                //Int32 pos = 0;
                double actPos = 0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueDouble(dbNo, key, "HNC_AXIS_ACT_POS", ref actPos);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, axdata.axisNo, ref pos, clientNo);
                if (ret != 0)
                {
                    return false;
                }
                axdata.actPos = actPos;
                return true;
            }
        }

        class CmdPosCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                Int32 ret = 0;
                //Int32 pos = 0;
                double cmdPos = 0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueDouble(dbNo, key, "HNC_AXIS_CMD_POSS", ref cmdPos);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS, axdata.axisNo, ref pos, clientNo);
                if (ret != 0)
                {
                    return false;
                }

                axdata.cmdPos = cmdPos;

                return true;
            }
        }

        class FollowErrCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                Int32 ret = 0;
                double followErr = 0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueDouble(dbNo, key, "HNC_AXIS_FOLLOW_ERR", ref followErr);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_FOLLOW_ERR, axdata.axisNo, ref axdata.followErr, clientNo);
                if (ret != 0)
                {
                    return false;
                }
                axdata.followErr = followErr;
                return true;
            }
        }

        class SvCurrentCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                int ret = 0;
                double svCurrent = 0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueDouble(dbNo, key, "HNC_AXIS_RATED_CUR", ref svCurrent);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, axdata.axisNo, ref svCurrent, clientNo);
                if (ret == 0)
                {
                    axdata.svCurrent = svCurrent;
                }
                return (ret == 0 ? true : false);
            }
        }

        class LoadCurrentCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                int ret = 0;
                double loadCurrent = 0;
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueDouble(dbNo, key, "HNC_AXIS_LOAD_CUR", ref loadCurrent);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, axdata.axisNo, ref loadCurrent, clientNo);
                if (ret == 0)
                {
                    axdata.loadCurrent = loadCurrent;
                }
                return (ret == 0 ? true : false);
            }
        }

        class DrvVerCollector : CollectDeviceAXData
        {
            public override bool GatherData(AxisData axdata, int dbNo)
            {
                int ret = 0;
                string drvVer = "";
                string key = "Axis:" + axdata.axisNo;
                ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, key, "HNC_AXIS_DRIVE_VER", ref drvVer);
                //ret = MacDataService.GetInstance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_DRIVE_VER, axdata.axisNo, ref drvVer, clientNo);
                if (ret == 0)
                {
                    axdata.drvVer = drvVer;
                }
                return (ret == 0 ? true : false);
            }
        }

        List<CollectDeviceAXData> axGatherlist;
        List<CollectDeviceAXData> axConstlist;
        AxisNameCollector axnameGather;
        AxisTypeCollector axisTypeGather;
//        AxisDistCollector distGather;
//        AxisPulseCollector pulseGather;
        ActPosCollector actPosGather;
        CmdPosCollector cmdPosGather;
        FollowErrCollector followErrGather;
        SvCurrentCollector svCurrentGather;
        LoadCurrentCollector loadCurrentGather;
        DrvVerCollector drvVerGather;

        public CollectAxisData()
        {
            axnameGather = new AxisNameCollector();
            axisTypeGather = new AxisTypeCollector();
//            distGather = new AxisDistCollector();
//            pulseGather = new AxisPulseCollector();
            actPosGather = new ActPosCollector();
            cmdPosGather = new CmdPosCollector();
            followErrGather = new FollowErrCollector();
            svCurrentGather = new SvCurrentCollector();
            loadCurrentGather = new LoadCurrentCollector();
            drvVerGather = new DrvVerCollector();

            axGatherlist = new List<CollectDeviceAXData>();    
            axGatherlist.Add(actPosGather);
            axGatherlist.Add(cmdPosGather);
            axGatherlist.Add(followErrGather);
            axGatherlist.Add(svCurrentGather);
            axGatherlist.Add(loadCurrentGather);
            axGatherlist.Add(drvVerGather);

            axConstlist = new List<CollectDeviceAXData>();
            axConstlist.Add(axnameGather);
            axConstlist.Add(axisTypeGather);
   //         axConstlist.Add(distGather);
   //         axConstlist.Add(pulseGather);
        }

        public override bool GatherData(AxisData  axdata,int dbNo)
        {
            bool result = true;
            foreach (CollectDeviceAXData temp in axGatherlist)
            {
                result = temp.GatherData(axdata, dbNo) && result;
            }

            return result;
        }

        public bool GatherConstData(AxisData axdata, int clientNo)
        {
            bool result = true;
            foreach (CollectDeviceAXData temp in axGatherlist)
            {
                result = temp.GatherData(axdata, clientNo) && result;
            }

            return result;
        } 

        ~CollectAxisData()
        {
            axGatherlist.Clear();
        }
    }

}