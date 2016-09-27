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
    public static class Client
    {
        private static string IP { get; set; }
        private static int Port { get; set; }
        
        public static int getPort()
        {
            return Port;
        }

        public static string getIP()
        {
            return IP;
        }
        
        // = new TcpClient();
        private static List<DbConnect.DbConnect.LogTableRecord> args;

        public static void init(string[] clientConfig)
        {
            //GET IP and PORT
            try
            {
                IP = clientConfig[0];
                Port = int.Parse(clientConfig[1]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Begin()
        {
            while (true)
            {
                args = DbConnect.DbConnect.GetQueries();
                if (args.Count > 0)
                    BeginSendingQueries();
                args = null;
            }
        }

        private static void BeginSendingQueries()
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

        private static void updateLog(DbConnect.DbConnect.LogTableRecord logRecord)
        {
            DbConnect.DbConnect.UpdateLog(logRecord);
            Console.WriteLine("query updated");
            Console.WriteLine("****************************");
        }

        private static void sendQuery(string query)
        {
            TcpClient client = new TcpClient(IP, Port);
            //TcpClient client = new TcpClient("192.168.0.103", 8888);

            //if (client.Connected)
            //{
                NetworkStream stream = client.GetStream();
                byte[] queryAsBytes = Encoding.ASCII.GetBytes(query);
                stream.Write(queryAsBytes, 0, queryAsBytes.Length);
            stream
                stream.Close();

                Console.WriteLine("query sent: " + query);
            //}
        }
    }
}
