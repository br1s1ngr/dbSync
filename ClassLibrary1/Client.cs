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
        private static TcpClient client;

        public static int getPort()
        {
            return Port;
        }

        public static string getIP()
        {
            return IP;
        }
        
        // = new TcpClient();
        private static List<DbConnect.DbConnect.LogTableRecord> queries;

        public static void init(string[] clientConfig)
        {
            //GET IP and PORT
            try
            {
                IP = clientConfig[0];
                Port = int.Parse(clientConfig[1]);
                //client = new TcpClient(IP, Port);
                connectClient();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private static void connectClient()
        {
            client = new TcpClient();
            client.Connect(IP, Port);
        }

        public static void Begin()
        {
            while (true)
            {
                queries = DbConnect.DbConnect.GetQueries();
                if (queries.Count > 0)
                {
                    if (client.Connected)
                        BeginSendingQueries();
                    else
                    {
                        Thread.Sleep(1500);
                        connectClient();
                    }
                }
                queries = null;
            }
        }

        private static void BeginSendingQueries()
        {
            bool errorOccurred = false;
            foreach (var record in queries)
            {
                if (errorOccurred)
                    break;

                try
                {
                    sendQuery(record.Argument);
                    if (recieveResponse())
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

        private static bool recieveResponse()
        {
            NetworkStream stream = client.GetStream();
            int bytesRecieved = 0;
            List<byte> listBYtes = new List<byte>();
            do {
                byte[] bytes = new byte[1024];
                bytesRecieved = stream.Read(bytes, 0, bytes.Length);
                listBYtes.AddRange(bytes.Take(bytesRecieved));
            } while (bytesRecieved != 0);
            //stream.Close();
            string msg = Encoding.ASCII.GetString(listBYtes.ToArray());
            if (msg == "recieved")
                return true;
            return false;
        }

        private static void updateLog(DbConnect.DbConnect.LogTableRecord logRecord)
        {
            DbConnect.DbConnect.UpdateLog(logRecord);
            Console.WriteLine("query updated");
            Console.WriteLine("****************************");
        }

        private static void sendQuery(string query)
        {
            //TcpClient client = new TcpClient(IP, Port);
            //TcpClient client = new TcpClient("192.168.0.103", 8888);

            //if (client.Connected)
            //{
                NetworkStream stream = client.GetStream();
                byte[] queryAsBytes = Encoding.ASCII.GetBytes(query);
                stream.Write(queryAsBytes, 0, queryAsBytes.Length);
                stream.Flush();
                //stream.Close();
                
            //    client.GetStream();

                Console.WriteLine("query sent: " + query);
            //}
        }
    }
}
