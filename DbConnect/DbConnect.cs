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

        public static void RunQuery(string sqlQuery)
        {
            //TO DO: get connection string details from file
            int i = 0;
            string connString = "server=" + Server + "; database= " + Database + "; uid=" + Uid + "; password=" + Pwd + ";";
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand(sqlQuery, conn);
                conn.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw ex;
                }
                //finally
                //{
                //    conn.Close();
                //}
                //if (i > 0)
                //    return true;
                //return false;
            }
        }

        public static List<LogTableRecord> GetQueriesFromGeneral()
        {
            List<LogTableRecord> queryList = new List<LogTableRecord>();
            //TO DO: get connection string details from file
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = " select * from general_log where argument NOT like '%mysql%' and argument NOT like '%general_log%' and copied = 0 and (argument like 'update%' or argument like 'insert%' or argument like 'delete%' or argument like 'create%' or argument like 'drop%' or argument like 'alter%' or argument like 'rename%' or argument like 'truncate%' or argument like '%transaction%'); ";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                DateTime eventTime = (DateTime)row.ItemArray[0];
                string userHost = row.ItemArray[1].ToString();
                string thread = row.ItemArray[2].ToString();
                string server = row.ItemArray[3].ToString();
                string arg = row.ItemArray[5].ToString();
                queryList.Add(new LogTableRecord(eventTime, userHost, arg, thread, server));
            }
            return queryList;
        }

        //public static void UpdateGeneralLog(LogTableRecord record)
        //{
        //    //TO DO: get connection string details from file
        //    string connString = "server=localhost; database=mysql; uid=root; password='';";
        //    using (MySqlConnection conn = new MySqlConnection(connString))
        //    {
        //        string query = "SET GLOBAL general_log = 'OFF'; RENAME TABLE general_log TO general_log_temp;";
        //        string time = record.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
        //        query += "update mysql.general_log_temp set uploaded=true where  event_time = '" + time + "' and thread_id = '" + record.ThreadID + "' and server_id = '" + record.ServerID + "';";
        //        query += "RENAME TABLE general_log_temp TO general_log; SET GLOBAL general_log = 'ON';";
        //        Console.WriteLine("**********************************");
        //        Console.WriteLine("running query to update log: " + query);
        //        Console.WriteLine("**********************************");

        //        MySqlCommand cmd = new MySqlCommand(query, conn);
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //}

        public static void UpdateGeneralLog(LogTableRecord record)
        {
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = "SET GLOBAL general_log = 'OFF'; RENAME TABLE general_log TO general_log_temp;";
            string time = record.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
            query += "update mysql.general_log_temp set uploaded=true where  event_time = @time and thread_id = @threadID and server_id = @serverID and argument = @arg and user_host=@user;";
            query += "RENAME TABLE general_log_temp TO general_log; SET GLOBAL general_log = 'ON';";

            Console.WriteLine("**********************************");
            Console.WriteLine("running query to update log: " + query);
            Console.WriteLine("**********************************");

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@arg", record.Argument);
            cmd.Parameters.AddWithValue("@threadID", record.ThreadID);
            cmd.Parameters.AddWithValue("@serverID", record.ServerID);
            cmd.Parameters.AddWithValue("@time", record.EventTime);
            cmd.Parameters.AddWithValue("@user", record.UserHost);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        //public static void SaveTimeHashSuccess(string eventTime, string hash)
        //{
        //    string connString = "server=localhost; database=server_db; uid=root; password='';";
        //    MySqlConnection conn = new MySqlConnection(connString);
        //    string query = "insert into server_db.log  values ('" + eventTime + "' , '" + hash + "');";
        //    MySqlCommand cmd = new MySqlCommand(query, conn);
        //    try
        //    {
        //        conn.Open();
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        conn.Close();
        //        throw ex;
        //    }
        //}

        public static void SaveQueryID(int id)
        {
            string connString = "server=localhost; database=server_db; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = "insert into server_db.query_log  values (" + id + ");";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        public static bool QueryInLog(/*string eventTime, string hash*/ int id)
        {
            //TryCreateServerLogDb();
            string connString = "server=localhost; database=server_db; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = "select count(*) from server_db.query_log where query_id = " + id + ";";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            try
            {
                conn.Open();
                string xx = cmd.ExecuteScalar().ToString();
                int i = int.Parse(xx);
                conn.Close();
                return (i > 0) ? true : false;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        public static void InsertQueryIntoClientLog(LogTableRecord record)
        {
            string connString = "server=localhost; database=client_db; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);

            MySqlCommand cmd = new MySqlCommand("insert into client_db (event_time, user_host, thread_id, server_id, argument) values (@event_time, @user_host, @thread_id, @server_id, @argument);", conn);
            cmd.Parameters.AddWithValue("@event_time", record.EventTime);
            cmd.Parameters.AddWithValue("@user_host", record.UserHost);
            cmd.Parameters.AddWithValue("@thread_id", record.ThreadID);
            cmd.Parameters.AddWithValue("@server_id", record.ServerID);
            cmd.Parameters.AddWithValue("@argument", record.Argument);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }

        public static List<ClientLogTableRecord> GetQueriesFromClientLog()
        {
            List<ClientLogTableRecord> queryList = new List<ClientLogTableRecord>();
            //TO DO: get connection string details from file
            string connString = "server=localhost; database=client_db; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = " select * from client_db where uploaded = 0; ";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                int id = (int)row.ItemArray[0];
                string arg = row.ItemArray[5].ToString();
                queryList.Add(new ClientLogTableRecord(arg, id));
            }
            return queryList;
        }

        public static void UpdateRecordInClientLog(int id)
        {
            string connString = "server=localhost; database=client_db; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand("update client_db set uploaded = 1 where id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                throw ex;
            }
        }



        public class LogTableRecord
        {
            public LogTableRecord(string query)
            {
                Argument = query;
            }

            public LogTableRecord(DateTime time, string userHost, string query, string thread, string server)
            {
                Argument = query; EventTime = time; ThreadID = thread; ServerID = server; UserHost = userHost;
            }

            public DateTime EventTime { get; set; }
            public string ThreadID { get; set; }
            public string ServerID { get; set; }
            public string Argument { get; set; }
            public string UserHost { get; set; }
        }


        public class ClientLogTableRecord
        {
            public string Query { get; set; }
            public int ID { get; set; }

            public ClientLogTableRecord(string query, int id)
            {
                Query = query;
                ID = id;
            }
        }
    }
}
