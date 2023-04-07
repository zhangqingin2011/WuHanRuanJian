using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HNCAPI;
using System.Windows.Forms;
using HNCAPI.Data;
using System.IO;

// net_to_redis
namespace HNC_MacDataService
{
    /// <summary>
    /// 读取redis内mac数据的类以及部分模仿二次开发接口的封装
    /// </summary>
    public class MacDataService
    {
        private ConnectionMultiplexer _LocalConnection;
        private static MacDataService _MacDataService;
        private const int CONNECT_TIMEOUT = 3;
        /// <summary>
        /// redis db的数量
        /// </summary>
        private const int DB_SUM = 24;
       
        #region 以下是读取redis的简单封装
        private MacDataService()
        {
            //_LocalDB = LocalRedisDB;
        }

        public static MacDataService GetInstance()
        {
            if (_MacDataService == null)
            {
                _MacDataService = new MacDataService();
            }
            return _MacDataService;
        }

        private StackExchange.Redis.ConnectionMultiplexer RedisConnectLocal
        {
            get
            {
                try
                {
                    if (_LocalConnection == null || !_LocalConnection.IsConnected)
                    {
                        _LocalConnection = ConnectionMultiplexer.Connect("127.0.0.1:6379,allowAdmin=true");

                    }
                    return _LocalConnection;
                }
                catch
                {
                    return null;
                }
            }
        }

        public StackExchange.Redis.IDatabase LocalDatabase
        {
            get
            {
                if (RedisConnectLocal != null)
                {
                    return RedisConnectLocal.GetDatabase(0);
                }
                return null;
            }
        }

        private StackExchange.Redis.IServer LocalServer
        {
            get
            {
                if (RedisConnectLocal != null)
                {
                    return RedisConnectLocal.GetServer("127.0.0.1", 6379);
                }
                return null;
            }
        }

        long timestamptemp = 0;

        public bool IsNCConnectToDatabase(Int32 dbNo)
        {
            bool result = false;
            try
            {
                if (RedisConnectLocal != null && dbNo >= 0)
                {
                    long timestamp = Convert.ToInt64(RedisConnectLocal.GetDatabase(dbNo).StringGet("TimeStamp"));
                    long ms = (System.DateTime.UtcNow.Ticks - new System.DateTime(1970, 1, 1).Ticks) / 10000- timestamp;
                    //if (ms < CONNECT_TIMEOUT*1000)
                        //result = true;
                    if (timestamp != timestamptemp)
                    {
                        timestamptemp = timestamp;
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("isNCConnect: " + ex.Message);
            }
            return result;
        }

        public int GetKeyValueString(int dbNo, String key, ref String value)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        value = this.RedisConnectLocal.GetDatabase(dbNo).StringGet(key);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetKeyValue----------" + ":" + ex.Message);
            }
            return ret;
        }

        public int GetHashKeyValueString(Int32 dbNo, String key, String hashField, ref String value)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).HashExists(key, hashField))
                    {
                        value = this.RedisConnectLocal.GetDatabase(dbNo).HashGet(key, hashField).ToString();
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("getHashKeyValueString" + ":" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 返回hash表中所有field的值
        /// </summary>
        /// <param name="dbNo"></param>
        /// <param name="key"></param>
        /// <returns>List</returns>
        public List<String> GetHashAllString(Int32 dbNo, String key)
        {
            List<String> values = new List<string>();
            try
            {
                if (this.LocalDatabase != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        HashEntry[] allHashEntry = this.RedisConnectLocal.GetDatabase(dbNo).HashGetAll(key);
                        for (int i = 0; i < allHashEntry.Length; i++)
                        {
                            values.Add(allHashEntry[i].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllValuesOfHashTable" + ":" + ex.Message);
            }
            return values;
        }

        public int GetHashKeyLength(Int32 dbNo, String key, ref long hashLength)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        hashLength = this.RedisConnectLocal.GetDatabase(dbNo).HashLength(key);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashKeyLength" + ":" + ex.Message);
            }
            return ret;
        }

        public int GetHashLength(Int32 dbNo, String key, ref int hashLength)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        RedisValue[] fieldNames = this.RedisConnectLocal.GetDatabase(dbNo).HashKeys(key);
                        if (fieldNames != null)
                        {
                            hashLength = fieldNames.Length;
                            ret = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetKeyByPatternLength" + ":" + ex.Message);
            }
            return ret;
        }

        public String[] GetHashFields(Int32 dbNo, String key)
        {
            String[] fields = null;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        RedisValue[] fieldNames = this.RedisConnectLocal.GetDatabase(dbNo).HashKeys(key);
                        if(fieldNames != null)
                        {
                            fields = Array.ConvertAll<RedisValue, String>(fieldNames, s => s.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashFields" + ":" + ex.Message);
            }
            return fields;
        }

        /// <summary>
        /// 获得文件夹下所有key
        /// </summary>
        /// <param name="dbNo"></param>
        /// <param name="FolderName"></param>
        /// <returns></returns>
        public String[] GetFolderKeys(int dbNo, String FolderName)
        {
            String[] FolderKeys = null;
            try
            {
                if (this.LocalDatabase != null)
                {
                    IEnumerable<StackExchange.Redis.RedisKey> keys = LocalServer.Keys(dbNo, FolderName + ":*");
                    StackExchange.Redis.RedisKey[] RedisKeys = keys.ToArray();
                    if (RedisKeys != null)
                    {
                        if (RedisKeys.Length > 0)
                        {
                            FolderKeys = new String[RedisKeys.Length];
                            for (int i = 0; i < RedisKeys.Length; i++)
                            {
                                FolderKeys[i] = RedisKeys[i].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetKeyValueHash" + ":" + ex.ToString());
            }
            return FolderKeys;
        }

        /// <summary>
        /// 通过ip获得CNC对应的dbNo
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="dbNo"></param>
        /// <returns></returns>
        public int GetMachineDbNo(String IP, ref int dbNo)
        {
            int ret = -1;
            for (int i = 0; i < DB_SUM; i++)
            {
                String IpInDb = "";
                if (GetKeyValueString(i, "IP", ref IpInDb) == 0)
                {
                    if (IP.Equals(IpInDb))
                    {
                        dbNo = i;
                        return 0;
                    }
                }
            }
            return ret;
        }

        public int GetHashKeyValueJson(Int32 dbNo, String key, String hashField, String param, ref String Json)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).HashExists(key, hashField))
                    {
                        String testString = this.RedisConnectLocal.GetDatabase(dbNo).HashGet(key, hashField).ToString();
                        var jObject = Newtonsoft.Json.Linq.JObject.Parse(testString);
                        Json = jObject[param].ToString();
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashKeyValueInt" + ":" + ex.Message);
            }
            return ret;
        }

        public int GetHashKeyValueInt(Int32 dbNo, String key, String hashField, ref int value)
        {
            string sValue = "";
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).HashExists(key, hashField))
                    {
                        sValue = this.RedisConnectLocal.GetDatabase(dbNo).HashGet(key, hashField).ToString();
                        value = Convert.ToInt32(sValue);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashKeyValueInt" + ":" + ex.Message);
            }
            return ret;
        }

        public int GetHashKeyValueDouble(Int32 dbNo, String key, String hashField, ref double value)
        {
            string sValue = "";
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).HashExists(key, hashField))
                    {
                        sValue = this.RedisConnectLocal.GetDatabase(dbNo).HashGet(key, hashField).ToString();
                        value = Convert.ToDouble(sValue);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashKeyValueDouble" + ":" + ex.Message);
            }
            return ret;
        }

        public int GetListKeyGcodeContent(Int32 dbNo, String key, ref List<string> GcodeContent)
        {
            int ret = -1;
            List<string> GCodeList = new List<string>();
            RedisValue[] GcodeTxt;

            long Gcodelength = 0;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        Gcodelength = this.RedisConnectLocal.GetDatabase(dbNo).ListLength(key);
                        if (Gcodelength > 0)
                        {
                            GcodeTxt = this.RedisConnectLocal.GetDatabase(dbNo).ListRange(key, 0, Gcodelength);
                            foreach (var a in GcodeTxt)
                            {
                                GCodeList.Add(a);
                            }
                            GcodeContent = GCodeList;
                            ret = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetListKeyGcodeContent" + ":" + ex.Message);
            }
            return ret;
        }



        public enum ToolParaIndex
        {
            GTOOL_DIR = 0,
            GTOOL_LEN1 = 1,
            GTOOL_LEN2 = 2,
            GTOOL_LEN3 = 3,
            GTOOL_LEN4 = 4,
            GTOOL_LEN5 = 5,
            GTOOL_RAD1 = 6,
            GTOOL_RAD2 = 7,
            GTOOL_ANG1 = 8,
            GTOOL_ANG2 = 9,
            GTOOL_TOTAL = 10,
            WTOOL_LEN1 = 24,
            WTOOL_LEN2 = 25,
            WTOOL_LEN3 = 26,
            WTOOL_LEN4 = 27,
            WTOOL_LEN5 = 28,
            WTOOL_RAD1 = 29,
            WTOOL_RAD2 = 30,
            WTOOL_ANG1 = 31,
            WTOOL_ANG2 = 32,
            WTOOL_TOTAL = 33,
            TETOOL_PARA0 = 48,
            TETOOL_PARA1 = 49,
            TETOOL_PARA2 = 50,
            TETOOL_PARA3 = 51,
            TETOOL_PARA4 = 52,
            TETOOL_PARA5 = 53,
            TETOOL_PARA6 = 54,
            TETOOL_PARA7 = 55,
            TETOOL_PARA8 = 56,
            TETOOL_PARA9 = 57,
            TETOOL_TOTAL = 58,
            EXTOOL_S_LIMIT = 72,
            EXTOOL_F_LIMIT = 73,
            EXTOOL_LARGE_LEFT = 74,
            EXTOOL_LARGE_RIGHT = 75,
            EXTOOL_TOTAL = 76,
            MOTOOL_TYPE = 96,
            MOTOOL_SEQU = 97,
            MOTOOL_MULTI = 98,
            MOTOOL_MAX_LIFE = 99,
            MOTOOL_ALM_LIFE = 100,
            MOTOOL_ACT_LIFE = 101,
            MOTOOL_MAX_COUNT = 102,
            MOTOOL_ALM_COUNT = 103,
            MOTOOL_ACT_COUNT = 104,
            MOTOOL_MAX_WEAR = 105,
            MOTOOL_ALM_WEAR = 106,
            MOTOOL_ACT_WEAR = 107,
            MOTOOL_GROUP = 108,
            MOTOOL_TOTAL = 109,
            METOOL_PARA0 = 120,
            METOOL_PARA1 = 121,
            METOOL_PARA2 = 122,
            METOOL_PARA3 = 123,
            METOOL_PARA4 = 124,
            METOOL_PARA5 = 125,
            METOOL_PARA6 = 126,
            METOOL_PARA7 = 127,
            METOOL_PARA8 = 128,
            METOOL_PARA9 = 129,
            METOOL_TOTAL = 130,
            INFTOOL_ID = 144,
            INFTOOL_MAGZ = 145,
            INFTOOL_CH = 146,
            INFTOOL_TYPE = 147,
            INFTOOL_STATE = 148,
            INFTOOL_G64MODE = 149,
            INFTOOL_TOTAL = 150,
            TOOL_PARA_TOTAL = 151,
        }

        #endregion

        #region 以下是仿HNCNET接口
        public int HNC_NetGetIpaddr(ref String ip, ref ushort port, int dbNo)
        {
            int ret = GetKeyValueString(dbNo, "IP", ref ip);
            String portStr = "";
            if (GetKeyValueString(dbNo, "Port", ref portStr) == 0)
            {
                try
                {
                    port = ushort.Parse(portStr);
                    ret = 0;
                }
                catch (Exception ex)
                {
                    ret = -1;
                    Console.WriteLine("HNC_NetGetIpaddr" + ":" + ex.Message);
                }
            }
            return ret;
        }

        public int HNC_SystemGetValue(int type, ref Int32 val, int dbNo)
        {
            String valStr = "";
            int ret = -1;
            switch (type)
            {
                case (int)HncSystem.HNC_SYS_ACCESS_LEVEL:
                    {
                        ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, "System", "SYS_ACCESS_LEVEL", ref valStr);
                    }
                    break;
                case (int)HncSystem.HNC_SYS_PLC_VER:
                    {
                        ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, "System", "SYS_PLC_VER", ref valStr);
                    }
                    break;
                default:
                    break;
            }
            if (ret == 0)
            {
                try
                {
                    val = Int32.Parse(valStr);
                }
                catch (Exception ex)
                {
                    ret = -1;
                    Console.WriteLine("HNC_SystemGetValue" + ":" + ex.Message);
                }
            }
            return ret;
        }

        public int HNC_SystemGetValue(int type, ref String val, int dbNo)
        {
            String valStr = "";
            int ret = -1;
            switch (type)
            {
               
                case (int)HncSystem.HNC_SYS_DRV_VER:
                    {
                        ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, "System", "SYS_DRV_VER", ref valStr);
                    }
                    break;
                default:
                    break;
            }

            if (ret == 0)
            {
                try
                {
                    val = valStr;
                    ret = 0;
                }
                catch (Exception ex)
                {
                    ret = -1;
                    Console.WriteLine("HNC_SystemGetValue" + ":" + ex.Message);
                }
            }
            return ret;
        }

        //public int HNC_RegGetValue(int type, int subtype, ref byte val, int dbNo)
        //{
        //    String valStr = "";
        //    int ret = -1;
        //    switch (type)
        //    {
        //        case (int)HncRegType.REG_TYPE_D:
        //            {
        //                ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, "System", "SYS_ACCESS_LEVEL", ref valStr);
        //            }
        //            break;
        //        case (int)HncSystem.HNC_SYS_PLC_VER:
        //            {
        //                ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, "System", "SYS_PLC_VER", ref valStr);
        //            }
        //            break;
        //        default:
        //            break;
        //    }

        //    try
        //    {
        //        val = Int32.Parse(valStr);
        //    }
        //    catch (Exception ex)
        //    {
        //        ret = -1;
        //        Console.WriteLine("HNC_SystemGetValue" + ":" + ex.Message);
        //    }
        //    return ret;
        //}
        //public int HNC_RegGetValue(int type, int subtype, ref short val, int dbNo)
        //{
        //    return 0;
        //}
        //public int HNC_RegGetValue(int type, int subtype, ref Int32 val, int dbNo)
        //{
        //    return 0;
        //}

        //-----------参数--------------
        public int HNC_ParamanGetI32(int filenum, int subclass, int id, ref int val, int dbNo)
        {
            String redisKey = "Parameter:" + ParameterDef.keyType[filenum];
            int ret = -1;
            Int32 propValue = -1;
            String json = "";
            String fields = "";
            fields = ParameterDef.GetParamanIdField(filenum, subclass, id);
            if (MacDataService.GetInstance().GetHashKeyValueJson(dbNo, redisKey, fields, "PropValue", ref json) == 0)
            {
                try
                {
                    propValue = int.Parse(json);
                    val = propValue;
                    ret = 0;
                }
                catch (Exception ex)
                {
                    ret = -1;
                    Console.WriteLine("HNC_ParamanGetI32: " + ex.Message);
                }
            }    
            return ret;
        }
        
        public int HNC_ParamanSetI32(int fileNo, int subNo, int index, int value, int dbNo)
        {
            return HNC_ParamanSetParaProp(fileNo, subNo, index, (byte)ParaPropType.PARA_PROP_VALUE, value, dbNo);
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value, int dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Parameter";
            m.Index = NCMessageFunction.PARAMAN_SET;// 0代表设置参数 1代表保存
            //m.SubType = prop_type.ToString();
            int dataType = HNCDATATYPE.DTYPE_BYTE;
            m.Value = "{\"fileNo\": " + filetype + ",\"subNo\": " + subid +
                ",\"index\": " + index + ",\"dataType\": " + dataType +
                ",\"prop_type\": " + prop_type + ",\"value\": \"" + Newtonsoft.Json.JsonConvert.SerializeObject(prop_value) + "\"}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ParamanSetParaProp" + ":" + ex.Message);
            }

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Double prop_value, int dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Parameter";
            m.Index = NCMessageFunction.PARAMAN_SET;// 0代表设置参数 1代表保存
            int dataType = HNCDATATYPE.DTYPE_FLOAT;
            m.Value = "{\"fileNo\": " + filetype + ",\"subNo\": " + subid +
                ",\"index\": " + index + ",\"dataType\": " + dataType +
                ",\"prop_type\": " + prop_type + ",\"value\": \"" + prop_value + "\"}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ParamanSetParaProp" + ":" + ex.Message);
            }

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Int32 prop_value, int dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Parameter";
            m.Index = NCMessageFunction.PARAMAN_SET; ;// 0代表设置参数 1代表保存
            int dataType = HNCDATATYPE.DTYPE_INT;
            m.Value = "{\"fileNo\": " + filetype + ",\"subNo\": " + subid +
                ",\"index\": " + index + ",\"dataType\": " + dataType +
                ",\"prop_type\": " + prop_type + ",\"value\": \"" + prop_value + "\"}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ParamanSetParaProp" + ":" + ex.Message);
            }

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, String prop_value, int dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Parameter";
            m.Index = NCMessageFunction.PARAMAN_SET; ;// 0代表设置参数 1代表保存
            int dataType = HNCDATATYPE.DTYPE_STRING;
            m.Value = "{\"fileNo\": " + filetype + ",\"subNo\": " + subid +
                ",\"index\": " + index + ",\"dataType\": " + dataType +
                ",\"prop_type\": " + prop_type + ",\"value\": \"" + prop_value + "\"}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ParamanSetParaProp" + ":" + ex.Message);
            }

            return ret;
        }

        public int HNC_ParamanSave(int dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Parameter";
            m.Index = NCMessageFunction.PARAMAN_SAVE; ;// 0代表设置参数 1代表保存
            m.Value = "";
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ParamanSetParaProp" + ":" + ex.Message);
            }

            return ret;
        }

        public int readFile(int dbNo)
        {
            int ret = -1;
            string readBitConent = "";
            string localName = "D:\\scada-1110\\scada-1110\\SCADA\\bin\\data\\health.SV";
           //  ret = MacDataService.GetInstance().GetHashKeyValueJson(dbNo, "Variables","System", "003637", ref readBitConent);
         //   List<String> varArray = MacDataService.GetInstance().HashGet(dbNo, "Variables:System");//Alarm:AlarmHistory
        
           // ret =dbNo.GetHashKeyValueString(dbNo, "Health:../tmp/historyIndexData.SV", "FileContent", ref readBitConent);
            // string to byte[]
            String[] bitArray = readBitConent.Split('-');
            byte[] writeBytes = new byte[bitArray.Length];
            for (int i = 0; i < bitArray.Length; i++)
                writeBytes[i] = Convert.ToByte(bitArray[i], 16);

            // 写文件
            FileStream fsWrite = new FileStream(localName, FileMode.Create);
            fsWrite.Write(writeBytes, 0, writeBytes.Length);
            fsWrite.Close();

            return ret;
           
        }
        //------------断刀-------------------//

        public class ToolBrokenInfo
        {
            public string Channel { get;set; }
            public string GCodeFile { get; set; }
            public int ToolNo { get; set; }
            public int IsBroken { get; set; }
        }     
       
        public int CancelToolBroken(int dbNo, bool Cancel)
        {
            int ret = -1;
           
            // 清除寄存器；
            //HncApi.HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 2626, 0, _ClientNo);
            //HncApi.HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 3013, 15, _ClientNo);

            // MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 2626, 0,0);
            // MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_G, 3013, 15,0);
        

            //try
            //{
            //    if (RedisConnectLocal != null)
            //    {
            //        //读ToolBrokenInfo，反序列化，修改IsBroken 


            //        if (GetKeyValueString(dbNo, "ToolBrokenInfo", ref ToolInfo) == 0)
            //        {
            //            // string Info = "{\"Channel\":\"123\",\"GCodeFile\":\"test\",\"ToolNo\":10,\"IsBroken\":false}";

            //            ToolBrokenInfo s = Newtonsoft.Json.JsonConvert.DeserializeObject<ToolBrokenInfo>(ToolInfo);                       
            //            if (s != null)
            //            {

            //      //          FeedbackInfo = "{\"Channel\": " + s.Channel + ",\"GCodeFile\": " + s.GCodeFile +
            //      //",\"ToolNo\": " + s.ToolNo + ",\"IsBroken\": " + Cancel + "}";

            //                //FeedbackInfo = "{\"Channel\":\"" + s.Channel + "\",\"GCodeFile\":\"" + s.GCodeFile +
            //                //       "\",\"ToolNo\":" + s.ToolNo + ",\"IsBroken\":" + IsBroken + "}";
            //                s.IsBroken = Cancel == true ? 0 : 1;
            //                FeedbackInfo = Newtonsoft.Json.JsonConvert.SerializeObject(s);
            //                if (LocalDatabase.KeyExists("ToolBrokenFeedback"))
            //                {
            //                    LocalDatabase.KeyDelete("ToolBrokenFeedback");
            //                }
            //                LocalDatabase.StringSet("ToolBrokenFeedback", FeedbackInfo);

            //                ret = 0;
            //            }
            //        }

            //    }
            //}

            HNCAPI.Data.NCMessage m = new HNCAPI.Data.NCMessage();
            m.Type = "Tool";
            m.Index = Cancel == true ? 0 : 1;    // 1 没断刀   // 0 断刀                    
            String message = m.ToString();
            m.Value = "";
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CancelToolBroken" + ":" + ex.Message);
            }

            return ret;
        }

        //测试断刀消息
        //public Int32 SentToolBrokenInfo(string Channel, string GCodeFile, int ToolNo, bool IsBroken, Int32 dbNo)
        //{
        //    int ret = -1;
        //    HNCAPI.Data.NCMessage m = new HNCAPI.Data.NCMessage();
        //    m.Type = "Tool";
        //   // m.Index = 0;// 0代表设置参数 1代表保存          
        //    m.Value = "{\"Channel\": " + Channel + ",\"GCodeFile\": " + GCodeFile +
        //        ",\"ToolNo\": " + ToolNo + ",\"IsBroken\": " + IsBroken + "\"}";
        //    String message = m.ToString();
        //    try
        //    {
        //        if (RedisConnectLocal != null)
        //        {
        //            String MachineSN = "";
        //            if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
        //            {
        //                ISubscriber sub = RedisConnectLocal.GetSubscriber();
        //                sub.Publish(MachineSN + ":SetValue", message);
        //                ret = 0;
        //            }
        //        }
        //    }
           
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("SentToolBrokenInfo" + ":" + ex.Message);
        //    }

        //    return ret;
        //}

        public Int32 HNC_GetRegValue(Int32 type, Int32 index, out int value, Int32 dbNo)
        {
            Int32 ret = -1;
            String RegType = "XYFGRWDBP";
            String key = "Register:" + RegType[type];
            String valueStr = "";
            ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, key, index.ToString("D4"), ref valueStr);
            if (ret == 0)
            {
                switch (type)
                {
                    case (int)HncRegType.REG_TYPE_X://x
                    case (int)HncRegType.REG_TYPE_Y://y
                    case (int)HncRegType.REG_TYPE_R://r
                        //byte value8 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value8, clientNo);
                        value = byte.Parse(valueStr);
                        break;
                    case (int)HncRegType.REG_TYPE_F://f
                    case (int)HncRegType.REG_TYPE_G://g
                    case (int)HncRegType.REG_TYPE_W://w
                        //Int16 value16 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value16, clientNo);
                        value = UInt16.Parse(valueStr);
                        break;
                    case (int)HncRegType.REG_TYPE_D://d
                    case (int)HncRegType.REG_TYPE_B://b
                    case (int)HncRegType.REG_TYPE_P://p
                        //Int32 value32 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value32, clientNo);
                        value = Int32.Parse(valueStr);
                        break;
                    default:
                        value = -1;
                        break;
                }
            }
            else
            {
                value = -1;
            }
            return ret;
        }

        //--------------寄存器---------------//
        public int HNC_RegClrBit(Int32 type, Int32 index, Int32 bit, Int32 dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Register";
            m.Index = NCMessageFunction.REG_CLR;
            String regType = "";
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    {
                        regType = "X";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    {
                        regType = "Y";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    {
                        regType = "F";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    {
                        regType = "G";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    {
                        regType = "R";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_W:
                    {
                        regType = "W";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_D:
                    {
                        regType = "D";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    {
                        regType = "B";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_P:
                    {
                        regType = "P";
                    }
                    break;
                default: break;
            }
            m.Value = "{\"regType\":\"" + regType + "\",\"index\": " + index +
               ",\"value\": " + bit + "}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_RegClrBit" + ":" + ex.Message);
            }

            return ret;
        }

        /*public Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit, Int32 dbNo)
        {
            Int32 value = 0;
            HNC_GetRegValue(type,index,out value,dbNo);

            return HNC_RegSetValue(type, index, (Int32)value | bit, dbNo);
        }*/
        
        public Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit, Int32 dbNo)
        {
            // TYPE= 寄存器类型;index=参数号,bit=位数
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Register";
            m.Index = NCMessageFunction.REG_SET_BIT;
            String regType = "";
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    {
                        regType = "X";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    {
                        regType = "Y";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    {
                        regType = "F";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    {
                        regType = "G";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    {
                        regType = "R";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_W:
                    {
                        regType = "W";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_D:
                    {
                        regType = "D";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    {
                        regType = "B";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_P:
                    {
                        regType = "P";
                    }
                    break;
                default: break;
            }
            m.Value = "{\"regType\":\"" + regType + "\",\"index\": " + index +
               ",\"value\": " + bit + "}";
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_RegSetBit" + ":" + ex.Message);
            }

            return ret;
        }

        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int16 value, Int32 dbNo)
        {
            return HNC_RegSetValue(type, index, (Int32)value, dbNo); 
        } 
        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Byte value, Int32 dbNo)
        {
            return HNC_RegSetValue(type, index, (Int32)value, dbNo); 
        }
        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int32 value, Int32 dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Register";
            m.Index = NCMessageFunction.REG_SET;
            String regType = "";
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    {
                        regType = "X";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    {
                        regType = "Y";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    {
                        regType = "F";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    {
                        regType = "G";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    {
                        regType = "R";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_W:
                    {
                        regType = "W";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_D:
                    {
                        regType = "D";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    {
                        regType = "B";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_P:
                    {
                        regType = "P";
                    }
                    break;
                default: break;
            }
            m.Value = "{\"regType\":\"" + regType + "\",\"index\": " + index +
               ",\"value\": " + value + "}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_RegSetBit" + ":" + ex.Message);
            }

            return ret;
        }


        //----------G代码-----------//
        public Int32 HNC_NetFileSend(String localName, String dstName, int dbNo)
        {
            string[] dstNamearr=null;
            string FileName = "";
            string key = "";
            string line = "";
            List<string> list = new List<string>();
            int ret = -1;

            if (localName != null)
            {
                //String dir = System.Windows.Forms.Application.StartupPath + "\\" + dbNo;
                // String localDir = dir + "\\" + localName;

                string localDir = localName;
                if (System.IO.File.Exists(localDir))
                {
                    System.IO.StreamReader content = new System.IO.StreamReader(localDir, Encoding.Default);

                    line = content.ReadLine();//按行读取G代码文件
                    while (line != null)
                    {
                        if (!line.Equals(""))
                            list.Add(line);
                        line = content.ReadLine();
                    }
                    // fileStream.Close();
                    //while ((line = content.ReadLine()) != null)
                    //{
                    //    list.Add(line);
                    //}
                    content.Close();

                    if (dstName != null)
                    {
                        if (dstName.Length > 0)
                        {
                            dstNamearr = dstName.Split('/');
                            FileName = dstNamearr[dstNamearr.Length - 1];
                            key = "GCodeFileSent:" + dstName;
                            WriteDataToLocalRedisDB(dbNo, key, list);
                            HNCAPI.Data.NCMessage m = new HNCAPI.Data.NCMessage();
                            m.Type = "File";
                            m.SubType = "h/lnc8/prog";
                           // m.SubType = "../prog";
                            m.Index = 0;   // Index=0 代表下发 ；Index=1 代表删除 ；Index=2 代表比较 ；
                            m.Value = FileName;
                            String message = m.ToString();
                            try
                            {
                                if (RedisConnectLocal != null)
                                {
                                    String MachineSN = "";
                                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                                    {
                                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                                        sub.Publish(MachineSN + ":SetValue", message);
                                        ret = 0;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("HNC_NetFileSend" + ":" + ex.Message);
                            }
                        }
                    }
                    return ret;
                }
            }
            return ret;  //往redis数据库写失败

        }
        public Int32 HNC_NetFileRemove(String dstName, Int16 dbNo)
        {
            int ret = -1;
            HNCAPI.Data.NCMessage m = new HNCAPI.Data.NCMessage();
            m.Type = "File";
            m.Index = 1;   // Index=0 代表下发 ；Index=1 代表删除 ；Index=2 代表比较 ；
            m.Value = dstName;
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("HNC_NetFileRemove" + ":" + ex.Message);
            }
            return ret;
        }
        public Int32 HNC_NetFileCheck(String localNamme, String dstName, Int16 dbNo)
        {
            int ret = -1;
            HNCAPI.Data.NCMessage m = new HNCAPI.Data.NCMessage();

            m.Type = "File";
            m.Index = 2;   // Index=0 代表下发 ；Index=1 代表删除 ；Index=2 代表比较 ；
            if (localNamme.Contains("\\"))
            {
                localNamme = localNamme.Split('\\')[localNamme.Split('\\').Length - 1];
            }

            m.Value = localNamme;
            m.SubType = dstName;
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("HNC_NetFileCheck" + ":" + ex.Message);
            }
            return ret;
        }
        public Int32 HNC_SysCtrlSelectProg(Int32 ch, String name, Int32 dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            int length = name.Length-7;
             name = ".." + name.Substring(7, length);
            m.Type = "SysCtrlProg";
            //m.Index = NCMessageFunction.REG_SET_BIT;
            m.Value = "{\"ch\":" + ch + ",\"name\":\"" + name + "\"}";
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_SysCtrlSelectProg" + ":" + ex.Message);
            }

            return ret;
        }

        //停止程序
        public Int32 HNC_SysCtrlStopProg(Int32 ch, Int32 dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "SysCtrlProg";
            m.Index = 1;
            m.Value = "{\"ch\":" + ch + "}";
            String message = m.ToString();
            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_SysCtrlStopProg" + ":" + ex.Message);
            }

            return ret;
        }
        //public Int32 HNC_SysCtrlSelectProg(Int32 ch, String dstName, int dbNo)
        //{
        //    int ret = -1;
        //    string key = "GCodeFile:" + dstName;
        //    string localNamme = "";
        //    if (dstName.Contains("/"))
        //    {
        //        localNamme = dstName.Split('/')[dstName.Split('/').Length - 1];
        //    }


        //    //List<string> GCodeList = new List<string>();
        //    RedisValue[] GcodeTxt;
        //    long Gcodelength = 0;
        //    try
        //    {
        //        if (RedisConnectLocal != null)
        //        {
        //            if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
        //            {
        //                Gcodelength = this.RedisConnectLocal.GetDatabase(dbNo).ListLength(key);
        //                GcodeTxt = this.RedisConnectLocal.GetDatabase(dbNo).ListRange(key, 0, Gcodelength);
        //                //foreach (var a in GcodeTxt)
        //                //{
        //                //    GCodeList.Add(a);
        //                //}
        //                // 写本地文件                       
        //                String dir = System.Windows.Forms.Application.StartupPath + "\\" + dbNo;
        //                String localDir = dir + "\\" + localNamme;
        //                if (System.IO.File.Exists(localDir))
        //                {
        //                    System.IO.File.Delete(localDir);
        //                }
        //                System.IO.StreamWriter wr = new System.IO.StreamWriter(localDir, true, Encoding.Default);
        //                for (int i = 0; i < GcodeTxt.Length; i++)
        //                {
        //                    wr.WriteLine(GcodeTxt[i].ToString());
        //                }
        //                wr.Close();
        //                ret = 0;
        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("HNC_SysCtrlSelectProg" + ":" + ex.Message);
        //    }
        //    return ret;
        //}
        private void WriteDataToLocalRedisDB(int dbno, string key, List<string> List)
        {
            // RedisKey Key;
            try
            {
                if (this.LocalDatabase != null)
                {
                    IBatch bacth = this.RedisConnectLocal.GetDatabase(dbno).CreateBatch();

                    if (this.RedisConnectLocal.GetDatabase(dbno).KeyExists(key))
                    {
                        this.RedisConnectLocal.GetDatabase(dbno).KeyDelete(key);
                    }
                    for (int i = 0; i < List.Count; i++)  //略去最后一行空格
                    {
                        bacth.ListRightPushAsync(key, List[i]);
                    }

                    bacth.Execute();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("UploadDataToRedisDB" + ":" + key + "  " + ex.StackTrace);
            }
            finally
            {
                //myNode.InUse = false;
            }

        }

        public Int32 HNC_ToolSetValue(Int32 type, Int32 toolNO, double toolVal, Int32 dbNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "ToolSet";

            m.Value = "{\"toolVal\":\"" + toolVal + "\",\"toolNO\": " + toolNO + ",\"type\": " + type + "}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(dbNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_ToolSet" + ":" + ex.Message);
            }

            return ret;
        }

        #endregion

        /*示例代码
         * 
         * MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_X, 0, 1, 0);
            MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_R, 0, 1, 0);
            MacDataService.GetInstance().HNC_ParamanSetI32(0, 0, 1, 2, 0);
            MacDataService.GetInstance().HNC_ParamanSetParaProp(0, 0, 2, (byte)ParaPropType.PARA_PROP_VALUE, 1, 0);
            MacDataService.GetInstance().HNC_ParamanSetParaProp(0, 0, 10, (byte)ParaPropType.PARA_PROP_VALUE, 1.25, 0);
            MacDataService.GetInstance().HNC_ParamanSetParaProp(0, 0, 35, (byte)ParaPropType.PARA_PROP_VALUE, "PROGX", 0);
            sbyte[] sbytes = { 0x0, 0x1, 0x1, 0x5};
            MacDataService.GetInstance().HNC_ParamanSetParaProp(1, 0, 33, (byte)ParaPropType.PARA_PROP_VALUE, sbytes, 0);
            MacDataService.GetInstance().HNC_ParamanSave(0);

            string localName = "D:\\project\\scada1029\\scada1028\\SCADA\\bin\\data\\CNCTask\\OS_CIR";
            string localName = "OS_AXIS";
            string dstName = "../prog/OS_AXIS";
            MacDataService.GetInstance().HNC_NetFileSend(localName, dstName, 0);//测试下发
            MacDataService.GetInstance().HNC_NetFileRemove(dstName, 0); //测试删除
            MacDataService.GetInstance().HNC_NetFileCheck(localName, dstName, 0); //测试删除
            MacDataService.GetInstance().HNC_ParamanSetI32(0, 1, 1, 1, 0);
            MacDataService.GetInstance().HNC_SysCtrlSelectProg(0, dstName, 0);
         */
    }
}
