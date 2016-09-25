using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Client
{
    public class Client
    {
        public string IP { get; set; }
        public int Port { get; set; }
        // = new TcpClient();
        List<DbConnect.DbConnect.LogTableRecord> args;

        public void init()
        { 
            while (true)
            {
                args = DbConnect.DbConnect.GetQueries();
                if (args.Count > 0)
                    BeginSendingQueries();
                args = null;
            }
        }

        private void BeginSendingQueries()
        {
            bool errorOccurred = false;
            foreach (var record in args)
            {
                if (errorOccurred)
                    break;

                try
                {
                    sendQuery(record.Argument);
                    updateLog(record);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    errorOccurred = true;
                }
            }
        }

        private void updateLog(DbConnect.DbConnect.LogTableRecord logRecord)
        {            
            DbConnect.DbConnect.UpdateLog(logRecord);
            Console.WriteLine("query updated");
            Console.WriteLine("****************************");
        }

        public void sendQuery(string query)
        {
            //TO DO: reading from config file comes here
            //TcpClient client = new TcpClient(IP, Port);
            TcpClient client = new TcpClient("192.168.0.103", 8888);

            //if (client.Connected)
            //{
                NetworkStream stream = client.GetStream();
                byte[] queryAsBytes = Encoding.ASCII.GetBytes(query);
                stream.Write(queryAsBytes, 0, queryAsBytes.Length);
                stream.Close();

                Console.WriteLine("query sent: " + query);
            //}
        }
    }
}
