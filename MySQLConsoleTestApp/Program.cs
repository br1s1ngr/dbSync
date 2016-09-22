using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<DateTime, string> queryResult = new Dictionary<DateTime, string>();

            List<Tuple<DateTime, string>> xxx = new List<Tuple<DateTime, string>>();

            string connString = "server=localhost; database=test; uid=root; password='';";
            MySqlConnection conn = new MySqlConnection(connString);
            string query = "select * from table1";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            conn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            string thisRow = "";
            
            while (reader.Read())
                for (int i = 0; i < reader.FieldCount; i++)
                    thisRow += reader.GetValue(i).ToString() + ",";
            conn.Close();

            //conn = new MySqlConnection(connString);
            //query = "insert into table1 values (null, 'test from condole app')";
            //cmd = new MySqlCommand(query, conn);
            //conn.Open();
            //cmd.ExecuteNonQuery();
            //conn.Close();

            connString = "server=localhost; database=mysql; uid=root; password='';";
            conn = new MySqlConnection(connString);
            query = " select * from general_log where argument NOT like '%mysql%' and argument like 'update%' or argument like 'insert%' or argument like 'delete%'; ";
            cmd = new MySqlCommand(query, conn);

            MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                string output = "";

                string arg = row.ItemArray[5].ToString();
                DateTime eventTime = (DateTime) row.ItemArray[0];

                output = arg + "\n" + eventTime;
                Console.WriteLine(output);
                Console.WriteLine();
            }

            //conn.Open();
            //reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine();
            //    string output = "";
            //    //for (int i = 0; i < reader.FieldCount; i++)
            //        output += reader.GetString("argument").ToString() + "\n" + reader.GetString("event_time").x();

            //        xxx.Add(new Tuple<DateTime, string>(DateTime.Parse(reader.GetString("event_time")), reader.GetString("argument")));

            //    //queryResult.Add(DateTime.Parse(reader.GetString("event_time")), reader.GetString("argument"));

            //    Console.WriteLine(output);
            //}
            //conn.Close();
                        
            Console.ReadLine();
        }
    }
}