using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace DbConnect
{
    public class DbConnect
    {
        private static string Database { get; set; }
        private static string Server { get; set; }
        private static string Uid { get; set; }
        private static string Pwd { get; set; }

        static string prevQuery;

        public static void init(string[] connectionInfo)
        {            
            Server = connectionInfo[0];
            Database = connectionInfo[1];
            Uid = connectionInfo[2];
            Pwd = connectionInfo[3];
        }

        public static bool RunQuery(string sqlQuery)
        {
            //TO DO: get connection string details from file
            int i = 0;
            string connString = "server=localhost; database=dbsynctest; uid=root; password='';";
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                conn.Open();
                
                try
                {
                    i = cmd.ExecuteNonQuery();
                    prevQuery = "";
                }
                catch (Exception ex)
                {
                    try
                    {
                        prevQuery += sqlQuery;
                        cmd = new MySqlCommand(prevQuery, conn);
                        i = cmd.ExecuteNonQuery();

                        prevQuery = "";
                    }
                    catch (Exception ex_II)
                    { }
                }
                finally
                {
                    conn.Close();
                }
                if (i > 0)
                    return true;
                return false;
            }
        }

        public static List<LogTableRecord> GetQueries()
        {
            List<LogTableRecord> queryList = new List<LogTableRecord>();
            //TO DO: get connection string details from file
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = " select * from general_log where argument NOT like '%mysql%' and argument NOT like '%general_log%' and uploaded = 0 and (argument like 'update%' or argument like 'insert%' or argument like 'delete%' or argument like 'create%' or argument like 'drop%' or argument like 'alter%' or argument like 'rename%' or argument like 'truncate%'); ";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string arg = row.ItemArray[5].ToString();
                DateTime eventTime = (DateTime)row.ItemArray[0];
                string thread = row.ItemArray[2].ToString();
                string server = row.ItemArray[3].ToString();
                queryList.Add(new LogTableRecord(eventTime, arg, thread, server));
            }
            return queryList;
        }

        public static void UpdateLog(LogTableRecord record)
        {
            //TO DO: get connection string details from file
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
            string query = "SET GLOBAL general_log = 'OFF'; RENAME TABLE general_log TO general_log_temp;";
            string time = record.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
            query += "update mysql.general_log_temp set uploaded=true where  event_time = '" + time + "' and thread_id = '" + record.ThreadID + "' and server_id = '" + record.ServerID + "';";
            query += "RENAME TABLE general_log_temp TO general_log; SET GLOBAL general_log = 'ON';";
                Console.WriteLine("**********************************");
                Console.WriteLine("running query to update log: " + query);
                Console.WriteLine("**********************************");

                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public class LogTableRecord
        {
            public LogTableRecord(string query)
            {
                Argument = query;
            }

            public LogTableRecord(DateTime time, string query, string thread, string server)
            {
                Argument = query; EventTime = time; ThreadID = thread; ServerID = server;
            }

            public DateTime EventTime { get; set; }
            public string ThreadID { get; set; }
            public string ServerID { get; set; }
            public string Argument { get; set; }
        }
    }
}
