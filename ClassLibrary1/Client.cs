using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Client
    {
        public string IP { get; set; }
        public int Port { get; set; }
        // = new TcpClient();
        List<Tuple<DateTime, string>> args;

        public void init()
        { 
            while (true)
            {
                args = DbConnect.DbConnect.GetQueries();
                BeginSendingQueries();
            }
        }

        private void BeginSendingQueries()
        {
            bool errorOccurred = true;
            foreach (var listItem in args)
            {
                if (errorOccurred)
                    break; 

                try
                {
                    sendQuery(listItem.Item2);
                    updateLog(listItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
                    errorOccurred = true;    
                }
            }
        }

        private void updateLog(Tuple<DateTime, string> listItem)
        {
            DbConnect.DbConnect dbconnect = new DbConnect.DbConnect(listItem.Item2, listItem.Item1);
            Thread dbThread = new Thread(new ThreadStart(dbconnect.UpdateLog));
        }

        public void sendQuery(string query)
        {
            
            TcpClient client = new TcpClient(IP, Port);
            //if (client.Connected)
            //{
                NetworkStream stream = client.GetStream();
                byte[] queryAsBytes = Encoding.ASCII.GetBytes(query);
                stream.Write(queryAsBytes, 0, queryAsBytes.Length);
                stream.Close();
            //}
        }
    }
}
