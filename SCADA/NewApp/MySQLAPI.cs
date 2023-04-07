using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace SCADA.NewApp
{
    public class WorkRack
    {
        public int NO;
        public string piecetype;
        public int status;
        public int loadflag;
    }

    public class MySQLAPI
    {
        private static MySQLAPI Operator;
        private static readonly object locker = new object();
        public static string strConn = "server=127.0.0.1;User Id=root;password=123456;Database=zibojishidb";
        private static readonly string[] dbTable = { "workrack", "processtask", "fittask", "cncbind", "statistics"};
        public enum DBTABLE
        {
            加工料仓表,
            加工订单表,
            装配订单表,
            机床绑定表,
            成品统计表
        }

        private MySQLAPI()
        {
        }

        public static MySQLAPI GetInstance()
        {
            lock (locker)
            {
                if (Operator == null)
                {
                    Operator = new MySQLAPI();
                }
            }
            return Operator;
        }

        public bool Addtoworkrack(int NO, string piecetype, int status, int loadflag)
        {
            bool res = false;
            string mysql = string.Format("INSERT INTO {0} (NO,piecetype,status, loadflag) VALUES('{1}','{2}','{3}','{4}')", dbTable[(int)DBTABLE.加工料仓表], NO, piecetype, status, loadflag);
            res = DBOperate(mysql);
            return res;
        }

        public bool UpdateByNOtoworkrack(int NO, string piecetype, int status, int loadflag)
        {
            bool res = false;
            string mysql = string.Format("UPDATE {0} SET piecetype='{1}', status='{2}', loadflag='{3}' WHERE NO ='{4}'", dbTable[(int)DBTABLE.加工料仓表], piecetype, status, loadflag, NO);
            res = DBOperate(mysql);
            return res;
        }

        public bool ExistNOtoworkrack(int NO)
        {
            bool res = false;
            string mysql = string.Format("select NO from {0} where NO='{1}'", dbTable[(int)DBTABLE.加工料仓表], NO);
            res = ExistFromTable(mysql);
            return res;
        }

        public Tuple<string, int, int> GetInfotoworkrack(int NO)
        {
            Tuple<string, int, int> data = null;
            string mysql = string.Format("select * from {0} where NO='{1}'", dbTable[(int)DBTABLE.加工料仓表], NO);
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    string piecetype = dr["piecetype"].ToString();
                    int status = int.Parse(dr["status"].ToString());
                    int loadflag = int.Parse(dr["loadflag"].ToString());
                    data = new Tuple<string, int, int>(piecetype, status, loadflag);
                }
                conn.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return data;
        }

        public bool Addtoprocesstask(int ordernum, int rackno, string piecetype, string Lprocess, string Cprocess, int status)
        {
            bool res = false;
            string mysql = string.Format("INSERT INTO {0} (ordernum, rackno, piecetype, Lprocess, Cprocess, status) VALUES('{1}','{2}','{3}','{4}','{5}','{6}')", dbTable[(int)DBTABLE.加工订单表],
                                        ordernum, rackno, piecetype, Lprocess, Cprocess, status);
            res = DBOperate(mysql);
            return res;
        }

        public bool Updatetoprocesstask(int ordernum, string Lprocess, string Cprocess, int status, string updatetime)
        {
            bool res = false;
            string mysql = string.Format("UPDATE {0} SET Lprocess='{1}', Cprocess='{2}', status='{3}', updatetime='{4}' WHERE ordernum ='{5}'", 
                                                dbTable[(int)DBTABLE.加工订单表], Lprocess, Cprocess, status, updatetime, ordernum);
            res = DBOperate(mysql);
            return res;
        }

        public bool Addtofittask(int ordernum, int Aracknum, int Bracknum, int status)
        {
            bool res = false;
            string mysql = string.Format("INSERT INTO {0} (ordernum, Aracknum, Bracknum, status) VALUES('{1}','{2}','{3}','{4}')", dbTable[(int)DBTABLE.装配订单表],
                                        ordernum, Aracknum, Bracknum, status);
            res = DBOperate(mysql);
            return res;
        }

        public bool Updatetofittask(int ordernum, int status, string updatetime)
        {
            bool res = false;
            string mysql = string.Format("UPDATE {0} SET status='{1}', updatetime='{2}' WHERE ordernum ='{3}'",
                                                dbTable[(int)DBTABLE.装配订单表], status, updatetime, ordernum);
            res = DBOperate(mysql);
            return res;
        }

        public bool FindDatatofittask(int Aracknum, int Bracknum, int status)
        {
            bool res = false;
            string mysql = string.Format("select * from {0} where Aracknum='{1}' and Bracknum='{2}' and status='{3}'", dbTable[(int)DBTABLE.装配订单表], Aracknum, Bracknum, status);
            res = ExistFromTable(mysql);
            return res;
        }

        public bool SelectTable(int tableindex, out DataTable table)
        {
            //table = new DataTable();
            bool res = false;
            string mysql = string.Format("select * from {0}", dbTable[tableindex]); 
            res = Select(mysql, out table);
            return res;
        }

        public bool FindMaxOrdernum(int tableindex, out int ordernum)
        {
            //ordernum = 0;
            bool res = false;
            string mysql = string.Format("select ordernum from {0} order by (ordernum) DESC", dbTable[tableindex]);
            res = FindInt(mysql, "ordernum", out ordernum);
            return res;
        }

        public bool FindMinNO(int tableindex, string piecetype, int loadflag, int status, out int NO)
        {
            NO = 0;
            bool res = false;
            string mysql = string.Format("select NO from {0} where piecetype='{1}' and loadflag='{2}' and status='{3}' order by (NO) ASC", dbTable[tableindex], piecetype, loadflag, status);
            res = FindInt(mysql, "NO", out NO);
            return res;
        }

        public bool FindOrdernumWhereStatus(int tableindex, int status, out List<int> list)
        {
            list = new List<int>();
            bool res = false;
            string mysql = string.Format("select ordernum from {0} where status={1}", dbTable[tableindex], status);
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    string num = dr["ordernum"].ToString();
                    list.Add(int.Parse(num));
                }
                conn.Close();
                res = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }

        public bool DeleteByOrdernum(int tableindex, int ordernum)
        {
            bool res = false;
            string mysql = string.Format("delete from {0} where ordernum={1}", dbTable[tableindex], ordernum);
            res = DBOperate(mysql);
            return res;
        }

        public bool GetCNCBind(string cnc, out int rack, out string piecetype)
        {
            rack = 0;
            piecetype = "无";
            bool res = false;
            string mysql = string.Format("select * from {0} where cnc='{1}'", dbTable[(int)DBTABLE.机床绑定表], cnc);
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                if(dr.Read())
                {
                    rack = int.Parse(dr["rackno"].ToString());
                    piecetype = dr["piecetype"].ToString();
                }
                conn.Close();
                res = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }

        public bool SetCNCBind(string cnc, int rackno, string piecetype)
        {
            bool res = false;
            string mysql = string.Format("UPDATE {0} SET  rackno='{1}', piecetype='{2}' WHERE cnc ='{3}'", dbTable[(int)DBTABLE.机床绑定表], rackno, piecetype, cnc);
            res = DBOperate(mysql);
            return res;
        }

        public bool AddToStatistics(string piecetype)
        {
            bool res = false;
            string mysql = string.Format("INSERT INTO {0} (piecetype) VALUES ('{1}')", dbTable[(int)DBTABLE.成品统计表], piecetype);
            res = DBOperate(mysql);
            return res;
        }

        public int CountToStatistics(string piecetype, DateTime startDate, DateTime endDate)
        {
            int count = 0;
            string mysql = string.Format("SELECT COUNT(piecetype) from {0} WHERE (piecetype = '{1}') AND (time > '{2}') AND (time < '{3}')", dbTable[(int)DBTABLE.成品统计表], piecetype, startDate, endDate);
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                var result = cmd.ExecuteScalar();
                count = Convert.ToInt32(result);
                conn.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return count;
        }

        private bool FindIntList(string mysql, string name, out List<int> vs)
        {
            vs = new List<int>();
            bool res = false;
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {                    
                    int value = int.Parse(dr[name].ToString());
                    vs.Add(value);
                }
                conn.Close();
                res = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }

        private bool FindInt(string mysql, string name, out int value)
        {
            value = 0;
            bool res = false;
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    res = true;
                    value = int.Parse(dr[name].ToString());
                }
                conn.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }

        private bool ExistFromTable(string mysql)
        {
            bool res = false;
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                DbDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    res = true;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }

        private bool Select(string sql, out DataTable table)
        {
            table = new DataTable();
            bool res = false;
            MySqlConnection mysqlconn = null;
            MySqlDataAdapter sda = null;
            try
            {
                mysqlconn = new MySqlConnection(strConn);
                sda = new MySqlDataAdapter(sql, mysqlconn);
                sda.Fill(table);
                res = true;
            }
            catch (Exception)
            {

            }
            finally
            {
                if (mysqlconn != null)
                {
                    mysqlconn.Close();
                }

                if (sda != null)
                {
                    sda.Dispose();
                }
            }
            return res;
        }

        private bool DBOperate(string mysql)
        {
            bool res = false;
            MySqlConnection conn = null;
            MySqlCommand cmd = null;
            try
            {
                conn = new MySqlConnection(strConn);
                conn.Open();
                cmd = new MySqlCommand(mysql, conn);
                int i = cmd.ExecuteNonQuery();
                conn.Close();
                res = true;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (cmd != null)
                {
                    cmd.Dispose();
                }
                if (conn != null)
                {
                    conn.Dispose();
                }
            }
            return res;
        }
    }
}
