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
        public string arg { get; set; }
        public DateTime eventTime { get; set; }

        public DbConnect(string query)
        {
            arg = query;
        }

        public DbConnect(string query, DateTime time)
        {
            arg = query; eventTime = time;
        }

        public void RunQuery()
        {
            string connString = "server=localhost; database=dbsynctest; uid=root; password='';";
            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                MySqlCommand cmd = new MySqlCommand(arg, conn);
                int i = cmd.ExecuteNonQuery();
            }
        }

        public static List<Tuple<DateTime, string>> GetQueries()
        {
            List<Tuple<DateTime, string>> queryList = new List<Tuple<DateTime, string>>();
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = " select * from general_log where argument NOT like '%mysql%' and argument like 'update%' or argument like 'insert%' or argument like 'delete%' ";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string arg = row.ItemArray[5].ToString();
                DateTime eventTime = (DateTime)row.ItemArray[0];
                queryList.Add(new Tuple<DateTime,string>(eventTime, arg));

                //string output = arg + "\n" + eventTime;
                //Console.WriteLine(output);
                //Console.WriteLine();
            }

            return queryList;
        }

        public void UpdateLog()
        {
            string connString = "server=localhost; database=mysql; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = "SET GLOBAL general_log = 'OFF'; RENAME TABLE general_log TO general_log_temp;";
            //query += "update mysql.general_log_temp set uploaded = true where argument = ";
            query += "delete from mysql.general_log_temp where argument = ";
            query += arg;
            query += "and event_time = '" + eventTime + "';";
            query += "RENAME TABLE general_log_temp TO general_log; SET GLOBAL general_log = 'ON';";

            Console.WriteLine(query);

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            


        }
    }
}
