using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Common;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace SCADA.Model
{
    public class TBaseTable:IDatabase
    {
        /// <summary>
        /// 向数据库插入数据
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public virtual async Task<string> InsesrtToDB(string connectStr)
        {
            string message = "insert to DB is OK";
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null ;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();

                List<Tuple<string, dynamic>> propertyList = GetProperiesAndValues();
                string nameText = string.Empty;
                string valueText = string.Empty;

                for (int i = 0; i < propertyList.Count; i++)
                {
                    if (i < propertyList.Count - 1)
                    {
                        nameText += propertyList[i].Item1 + ",";
                        valueText += "@" + propertyList[i].Item1 + ",";
                    }
                    else
                    {
                        nameText += propertyList[i].Item1;
                        valueText += "@" + propertyList[i].Item1;
                    }
                }
                string tableName = this.GetType().Name;
                command.CommandText = "insert into " + tableName + "(" +
                    nameText +
                    ") values (" +
                    valueText +
                    ")";
                foreach (var property in propertyList)
                {
                    command.Parameters.AddWithValue("@" + property.Item1 + "", property.Item2);
                }

                int excute = await command.ExecuteNonQueryAsync();
                connect.Close();
                return message;
            }
            catch(Exception ex)
            {
                message = "insert to DB failed:"+ex.ToString();
                return message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }            
        }

        /// <summary>
        /// 删除所有数据，谨慎使用，不可用于PositionCanUse，StockStatus
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public async Task<string> DeleteAllData(string connectStr)
        {
            string message = "delete all data is OK";
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                string tableName = this.GetType().Name;

                command.CommandText = "delete from " + tableName + "";
                int excute = await command.ExecuteNonQueryAsync();
                connect.Close();
                return message;
            }
            catch (Exception ex)
            {
                message = "delete all data failed:" + ex.ToString();
                return message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 根据条件删除任务
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<string> DeleteDataByCondition(string connectStr, string condition)
        {
            string message = "delete data is OK";
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();

                string tableName = this.GetType().Name;

                command.CommandText = "delete from " + tableName + " where " + condition + "";
                int excute = await command.ExecuteNonQueryAsync();
                connect.Close();
                return message;
            }
            catch (Exception ex)
            {
                message = "delete data failed:" + ex.ToString();
                return message;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取class的属性和属性值
        /// </summary>
        /// <returns></returns>
        public virtual List<Tuple<string, dynamic>> GetProperiesAndValues()
        {
            List<Tuple<string, dynamic>> properiesAndValuesList = new List<Tuple<string, dynamic>>();

            if (this == null)
            {
                throw new  ArgumentException("没有实例对象");
            }

            System.Reflection.PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

            if (properties.Length == 0) 
            {
                throw new ArgumentException("无法获取此类型的属性");
            }

            foreach (var property in properties)
            {
                string propertyName = property.Name;
                dynamic value = property.GetValue(this, null);

                Tuple<string, dynamic> propertyTuple = new Tuple<string, dynamic>(propertyName, value);

                properiesAndValuesList.Add(propertyTuple);
            }

            return properiesAndValuesList;
        }

    }

   
    public class MetrialMap
    {
        public static Dictionary<string, System.Drawing.Bitmap> MetrialMaps = new Dictionary<string, System.Drawing.Bitmap>();
        static MetrialMap()
        {
            MetrialMaps.Add("NoneeNonee", global::SCADA.Properties.Resources.无料); //空闲

            MetrialMaps.Add("FrontBlank", global::SCADA.Properties.Resources.前端盖未加工); //前端盖毛坯
            MetrialMaps.Add("FrontWorth", global::SCADA.Properties.Resources.前端盖合格); //前端盖合格
            MetrialMaps.Add("FrontWaste", global::SCADA.Properties.Resources.前端盖不合格); //前端盖不合格

            MetrialMaps.Add("TbackBlank", global::SCADA.Properties.Resources.后端盖未加工); //后端盖毛坯
            MetrialMaps.Add("TbackWorth", global::SCADA.Properties.Resources.后端盖合格); //后端盖合格
            MetrialMaps.Add("TbackWaste", global::SCADA.Properties.Resources.后端盖不合格); //后端盖不合格

            MetrialMaps.Add("RolloBlank", global::SCADA.Properties.Resources.轴类未加工); //轴类毛坯
            MetrialMaps.Add("RolloWorth", global::SCADA.Properties.Resources.轴类合格); //轴类合格
            MetrialMaps.Add("RolloWaste", global::SCADA.Properties.Resources.轴类不合格); //轴类不合格

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExcuteDBAction
    {
        /*惠普的笔记本电脑和武软的工控机装的Navicat中Mysql的版本不一样，所用的查询语句有别
         * 惠普电脑用signed
         * 武软工控机用INTEGER
        */
        //惠普电脑用字段
        private static string sqladapter = "signed";
        //武软工控机用字段
        //private static string sqladapter = "INTEGER";

        /// <summary>
        /// 打开时间调度器
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<string> ExecuteStartEvent(string connectStr)
        {
            string msg = "start event is OK";
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "SET GLOBAL event_scheduler=1;";
                int execute = await command.ExecuteNonQueryAsync();
                connect.Close();
            }
            catch (Exception ex)
            {
                return msg = "start event is failed:" + ex.ToString();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
            return msg;
        }

        public static async Task<int> ClearAllStatus(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "UPDATE `stockstatus` SET `MetrialType`='nonee', `MetrialQuality`='nonee'";
                int i = await command.ExecuteNonQueryAsync();
                command.CommandText = "UPDATE `positionstatus` SET `MetrialType`='nonee', `StatusCode`='0'";
                i = await command.ExecuteNonQueryAsync();
                command.CommandText = "UPDATE `unloadpostitionstatus` SET `StatusCode`='0'";
                i = await command.ExecuteNonQueryAsync();
                connect.Close();
                return i;
            }
            catch (Exception)
            {
                connect.Close();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }
        /// <summary>
        /// 将未完成的任务清空
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static int UnexecutedToCheck(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "DELETE from agv_task where STATUS = 0";
                int i = command.ExecuteNonQuery();
                connect.Close();
                return i;
            }
            catch (Exception)
            {
                connect.Close();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 料仓堆垛机接收到反馈的任务清空
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<int> ExecutingToCheck(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "DELETE from agv_task where STATUS = 1";
                int i = await command.ExecuteNonQueryAsync();
                connect.Close();
                return i;
            }
            catch (Exception)
            {
                connect.Close();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static async Task<int> UnusualExecutedToCheck(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "DELETE from agv_task where STATUS > 2";
                int i = await command.ExecuteNonQueryAsync();
                connect.Close();
                return i;
            }
            catch (Exception)
            {
                connect.Close();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        //public static 
        /// <summary>
        /// PositionID,MetrialType,MetrialQuality,MetrialCode，获取仓位状态（位置，类型，编码）
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="stackTable"></param>
        /// <returns></returns>
        public static async Task<List<Tuple<string, string, string,string>>> GetAllStatus(string connectStr)
        {
            List<Tuple<string, string, string, string>> statusList = new List<Tuple<string, string, string, string>>();
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                //string tableName = stackTable.GetType().Name;
                command.CommandText = "select *from StockStatus order by CAST(PositionID AS " + sqladapter + ") asc";

                DbDataReader dr = await command.ExecuteReaderAsync();
                while(dr.Read())
                {
                    string positionID = dr["PositionID"].ToString();
                    string metrialType = dr["MetrialType"].ToString();
                    string metrialQuality = dr["MetrialQuality"].ToString();
                    string metrialCode = dr["MetrialCode"].ToString();

                    Tuple<string, string, string, string> temp = new Tuple<string, string, string, string>(positionID, metrialType, metrialQuality, metrialCode);
                    statusList.Add(temp);
                }
                connect.Close();
                return statusList;
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
                return statusList;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 上料或入库时寻找空位
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static async Task<string>FindPosition(string connectStr)
        {
            string position = string.Empty;

            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                string findStr = "select *from PositionStatus where  StatusCode='0' order by CAST(PositionID AS " + sqladapter + ") asc";
                command.CommandText = findStr;
                DbDataReader dr = await command.ExecuteReaderAsync();
                if(dr.Read())
                {
                    position = dr["PositionID"].ToString();
                }
                connect.Close();
                return position;
            }
            catch(Exception ex)
            {
                MessageBox.Show("FindPosition failed:" + ex.ToString());
                return "Find position failed:" + ex.ToString();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static void UpdateUnloadStatusByagv_taskP2andstatus(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "UPDATE unloadpostitionstatus SET StatusCode = 1 WHERE PositionID in(SELECT p1 FROM agv_task WHERE p2 = '102' and status = 0)";
                command.ExecuteNonQuery();
                connect.Close();
            }
            catch (Exception)
            {
                connect.Close();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static void UpdateUnloadStatusByagv_taskID(string connectStr, string ID)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "UPDATE unloadpostitionstatus SET StatusCode = 1 WHERE PositionID in(SELECT p1 FROM agv_task WHERE ID = '" + ID + "')";
                command.ExecuteNonQuery();
                connect.Close();
            }
            catch (Exception)
            {
                connect.Close();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static void UpdateTaskStatusByID(string connectStr, string ID, int status)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "UPDATE agv_task SET status=" + status.ToString() + " WHERE (ID='" + ID + "')";
                command.ExecuteNonQuery();
                connect.Close();
            }
            catch (Exception)
            {
                connect.Close();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static string GetP2ByID(string connectStr, string ID)
        {
            string P2 = String.Empty;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                string findStr = "SELECT (p2) FROM agv_task WHERE ID = '"+ ID + "'";
                command.CommandText = findStr;
                DbDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    ID = dr["p2"].ToString();
                }
                connect.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
            return P2;
        }

        public static string FindErrorRackNoTaskListHeaderID(string connectStr, int errorRackNo)
        {
            string ID = String.Empty;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                string findStr = "SELECT (ID) FROM agv_task WHERE p1= '" + errorRackNo.ToString() + "' AND status != 2 ORDER BY (ID) DESC";
                command.CommandText = findStr;
                DbDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    ID = dr["ID"].ToString();
                }
                connect.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
            return ID;
        }

        public static bool FindExecutingTask(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "select (ID) from agv_task where status = 1";
                DbDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    connect.Close();
                    return true;
                }
                else
                {
                    connect.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static bool FindUnusualExecutedTask(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "select (ID) from agv_task where status > 2";
                DbDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    connect.Close();
                    return true;
                }
                else
                {
                    connect.Close();
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 是否可以上料
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<bool> CanLoadToStock(string connectStr, string position)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (ID) from agv_task where p2='100' and status<'2' and p1='" + position + "'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (!dr.Read())
                {
                    connect.Close();
                    return true;
                }
                else
                {
                    connect.Close();
                    return false;
                }
            }
            catch(Exception)
            {
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }
        /// <summary>
        /// 字符串是否可转换成int
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool PositonCanConvertToInt(string position)
        {
            try
            {
                int positionNum = int.Parse(position);
                return true;
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 判断agv_task表里面是否有未完成的入库任务，只能有一条正在执行的入库任务
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<bool> PutTaskIsOK(string connectStr)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                string searchStr = "select *from agv_task where priority='1' and status='1'";

                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = searchStr;
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    connect.Close();
                    return false;
                }
                else
                {
                    connect.Close();
                    return true;
                }
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
                connect.Close();
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 查找料架上的物料，毛坯，合格，不合格
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="type"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static async Task<string>FindMetrialPostition(string connectStr,string type,string quality)
        {
            string posionId = string.Empty;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (PositionID) from StockStatus where MetrialType='" + type + "' and MetrialQuality='" + quality + "' order by CAST(PositionID AS " + sqladapter + ") asc";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if(dr.Read())
                {
                    posionId = dr["PositionID"].ToString();
                }
                connect.Close();
                return posionId;
            }
            catch(Exception ex)
            {
                string s = ex.ToString();
                return s;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }


        /// <summary>
        /// 查找料架上的物料，毛坯，合格，不合格的集合
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="type"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static async Task<List<string>> FindMetrialPostitionList(string connectStr, string type, string quality)
        {
            List<string> posionIdList = new List<string>();
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (PositionID) from StockStatus where MetrialType='" + type + "' and MetrialQuality='" + quality+ "'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                while(dr.Read())
                {
                    string posionId = dr["PositionID"].ToString();
                    posionIdList.Add(posionId);
                }
                connect.Close();
                return posionIdList;
            }
            catch (Exception ex)
            {
                string s = ex.ToString();
                return posionIdList;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 查找多个符合条件的物料位置
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="type">类型</param>
        /// <param name="quality">质量</param>
        /// <param name="amount">下单数量</param>
        /// <returns>物料位置列表</returns>
        public async static Task<List<string>>FindManyMetrialPositions(string connectStr,string type,string quality,int amount)
        {
            List<string> positions = new List<string>();
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null; 
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();

                command.CommandText = "select count(*) from StockStatus where MetrialType='" + type + "' and MetrialQuality='" + quality + "' ";
                int count = await command.ExecuteNonQueryAsync();
                if(count<amount)
                {
                    MessageBox.Show("符合条件的物料只有：" + count + ",请重新下单");
                    connect.Close();
                    return null;
                }
                else
                {
                    command.CommandText = "select *from StockStatus where MetrialType='" + type + "' and MetrialQuality='" + quality + "' order by CAST(PositionID AS " + sqladapter + ") asc limit " + amount + "";

                    DbDataReader dr = await command.ExecuteReaderAsync();
                    while(dr.Read())
                    {
                        positions.Add(dr["PositionID"].ToString());
                    }
                    connect.Close();
                    return positions;
                }               
            }
            catch(Exception ex)
            {
                MessageBox.Show("FindManyMetrialPositions:" + ex.ToString());
                connect.Close();
                return null;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 将字符串首字母转换成大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertFirstToUpper(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length > 1)
                {
                    return char.ToUpper(str[0]) + str.Substring(1);
                }
                return char.ToUpper(str[0]).ToString();
            }
            return null;
        }

        /// <summary>
        /// 查找agv_task表中ID最大的数
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<string>FindMaxTaskID(string connectStr)
        {
            string maxID = string.Empty;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "SELECT (ID) FROM agv_task ORDER BY CAST(ID AS " + sqladapter + ") DESC";

                DbDataReader dr = await command.ExecuteReaderAsync();
                if(dr.Read())
                {
                    maxID = dr["ID"].ToString();
                }
                else
                {
                    maxID = "0";
                }
                connect.Close();
                return maxID;
            }
            catch (Exception ex)
            {
                connect.Close();
                return "search ID failed:"+ex.ToString();
            }
            finally
            {
                if (command != null)               
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 下料时判断任务是否已下过
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static async Task<bool>PositionIsOccupied(string connectStr,string position)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select *from agv_task where p1='" + position + "' and status!='2' and p2='101'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    connect.Close();
                    return false;
                }
                else
                {
                    connect.Close();
                    return true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("PositionIsOccupied failed:" + ex.ToString());
                connect.Close();
                return false;
            }
            finally
            {
                if(command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 查看入库指令发送到数据库没有
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static async Task<bool> WarehousinghasnotSend(string connectStr, string position)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select *from agv_task where p1='" + position + "' and status<'2' and p2='103'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    connect.Close();
                    return false;
                }
                else
                {
                    connect.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PositionIsOccupied failed:" + ex.ToString());
                connect.Close();
                return false;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }      

        /// <summary>
        /// 向plc写值
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="name"></param>
        /// <param name="statusValue"></param>
        /// <returns></returns>
        public static async Task<string> WriteToPlcdt(string connectStr,string name,int statusValue)
        {
            string message = "WriteToPlcdt is OK";
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "update plcdt set status='" + statusValue + "' where plcdt.name='" + name + "'";
                int ret = await command.ExecuteNonQueryAsync();
                connect.Close();
                return message;

            }
            catch(Exception ex)
            {
                //MessageBox.Show("WriteToPlcdt failed:" + ex.ToString());
                return "WtriteToPlcdt is falied:" + ex.ToString();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose(); 
                }
            }
        }

        /// <summary>
        /// 读plc
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<int>ReadFromPldt(string connectStr,string name)
        {
            int statusValue = 0;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (status) from plcdt where plcdt.name='" + name + "'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if(dr.Read())
                {
                    statusValue = int.Parse(dr["status"].ToString());
                }
                connect.Close();
                return statusValue;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static int ReadFromPldtQ(string connectStr, string name)
        {
            int statusValue = 0;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "select (status) from plcdt where plcdt.name='" + name + "'";
                DbDataReader dr = command.ExecuteReader();
                if (dr.Read())
                {
                    statusValue = int.Parse(dr["status"].ToString());
                }
                connect.Close();
                return statusValue;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return -1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        public static void WriteToPlcdtQ(string connectStr, string name, int statusValue)
        {
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                connect.Open();
                command = connect.CreateCommand();
                command.CommandText = "update plcdt set status='" + statusValue + "' where plcdt.name='" + name + "'";
                int ret = command.ExecuteNonQuery();
                connect.Close();
            }
            catch (Exception)
            {
                connect.Close();
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 下料查找物料
        /// </summary>
        /// <param name="connectStr"></param>
        /// <param name="Type"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static async Task<List<string>>FindMetriales(string connectStr,string Type,int count)
        {
            if(count<=0)
            {
                MessageBox.Show("订单有误");
                return null;
            }
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            List<string> positionList = new List<string>();
            try
            {                
                await connect.OpenAsync();

                command = connect.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM unloadpostitionstatus WHERE PositionID in(SELECT PositionID FROM stockstatus WHERE MetrialType='" + Type + "' AND MetrialQuality='blank') AND StatusCode='1'";

                var records = await command.ExecuteScalarAsync();
                if(Convert.ToInt32(records)<count)
                {
                    MessageBox.Show("没有这么多可用物料");
                    connect.Close();
                    return null;
                }
                else
                {
                    command.CommandText = "SELECT positionID from stockstatus WHERE PositionID in (SELECT PositionID from unloadpostitionstatus WHERE StatusCode='1') AND MetrialQuality='blank' AND MetrialType='" + Type + "' ORDER BY CAST(ID AS " + sqladapter + ") asc limit " + count + "";
                    DbDataReader dr = await command.ExecuteReaderAsync();
                    while(dr.Read())
                    {
                        positionList.Add(dr["PositionID"].ToString());
                        using (var connectTemp = new MySqlConnection(connectStr))
                        {
                            await connectTemp.OpenAsync();
                            var commandTemp = connectTemp.CreateCommand();
                            string updateStr = "update unloadpostitionstatus set StatusCode='0' where positionID='" + dr["PositionID"].ToString() + "'";
                            commandTemp.CommandText = updateStr;
                            await commandTemp.ExecuteNonQueryAsync();
                        }
                        /*
                        string updateStr = "update unloadPositionstatus set StatusCode='0' where positionID='" + dr["PositionID"].ToString() + "'";
                        command.CommandText = updateStr;
                        await command.ExecuteNonQueryAsync();
                         */
                    }
                    connect.Close();
                    return positionList;
                }

                //return null;
            }
            catch(Exception ex)
            {
                connect.Close();
                return null;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }
        /// <summary>
        /// 获取正在执行任务的库位号
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<string> GetDoingTaskP1(string connectStr)
        {
            string resP1 = null;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (p1) from agv_task where status='1'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    resP1 = dr["p1"].ToString();
                }
                connect.Close();
                return resP1;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return resP1;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取正在执行任务的任务类型
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<string> GetDoingTaskP2(string connectStr)
        {
            string resP2 = null;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (p2) from agv_task where status='1'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    resP2 = dr["p2"].ToString();
                }
                connect.Close();
                return resP2;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return resP2;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }

        /// <summary>
        /// 获取正在执行任务的料类型
        /// </summary>
        /// <param name="connectStr"></param>
        /// <returns></returns>
        public static async Task<string> GetDoingTaskP3(string connectStr)
        {
            string resP3 = null;
            MySqlConnection connect = new MySqlConnection(connectStr);
            MySqlCommand command = null;
            try
            {
                await connect.OpenAsync();
                command = connect.CreateCommand();
                command.CommandText = "select (p3) from agv_task where status='1'";
                DbDataReader dr = await command.ExecuteReaderAsync();
                if (dr.Read())
                {
                    resP3 = dr["p3"].ToString();
                }
                connect.Close();
                return resP3;
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
                return resP3;
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                }
                if (connect != null)
                {
                    connect.Dispose();
                }
            }
        }
    }

    public enum MType
    {
        Nonee, //无料
        Front, //前端盖
        Tback, //后端盖
        Rollo //轴类
    }

    public enum MQuality
    {
        Nonee, //无料
        Blank, //毛坯
        Worth, //合格
        Waste //不合格·
    }

    public enum CommandCode
    {
        BlankInput=100, //上料
        MetrialOut=101, // 下料
        MetrialDelivery=102,//出库
        MetrialInStock=103 //入库        
    }
    
}
