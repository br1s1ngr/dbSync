using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Client
{
    public static class Client
    {
        private static string IP { get; set; }
        private static int Port { get; set; }
        static Socket clientSocket;
        static Encoding code = Encoding.ASCII;
        static string delimeter = "_end_";
        static string storage;

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
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(IP, Port);
        }

        public static void Begin()
        {
            Console.WriteLine("Connecting......");
            Console.WriteLine();
            while (true)
            {
                queries = DbConnect.DbConnect.GetQueries();
                if (queries.Count > 0)
                {
                    if (clientSocket.Connected)
                        BeginSendingQueries();
                    else
                    {
                        Thread.Sleep(1500);
                        connectClient();
                    }
                }
                Thread.Sleep(100);
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
                    sendQuery(record);
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
            storage = "";
            byte[] rgb = new byte[8192];
            int byteCount = 0;

            while ((byteCount = clientSocket.Receive(rgb)) > 0)
                recieveBytes(rgb, byteCount);

            if (storage == delimeter)
                return true;

            return false;
            //NetworkStream stream = client..GetStream();
            //client.
            //  int bytesRecieved = 0;
            //  List<byte> listBYtes = new List<byte>();
            //  do {
            //      byte[] bytes = new byte[1024];
            //      bytesRecieved = stream.Read(bytes, 0, bytes.Length);
            //      listBYtes.AddRange(bytes.Take(bytesRecieved));
            //  } while (stream.DataAvailable);
            ////stream.Close();
            //  string msg = Encoding.ASCII.GetString(listBYtes.ToArray());
            //  if (msg == "recieved")
            //  {
            //      Console.WriteLine("************************");
            //      Console.WriteLine("response recieved");
            //      Console.WriteLine("************************");

            //      return true;
            //  }
            //return false;
        }

        private static void recieveBytes(byte[] rgb, int byteCount)
        {
            storage += code.GetString(rgb, 0, byteCount);
        }

        private static void updateLog(DbConnect.DbConnect.LogTableRecord logRecord)
        {
            DbConnect.DbConnect.UpdateLog(logRecord);
            Console.WriteLine("query updated");
            Console.WriteLine("****************************");
        }

        private static void sendQuery(DbConnect.DbConnect.LogTableRecord record)
        {
            //TcpClient client = new TcpClient(IP, Port);
            //TcpClient client = new TcpClient("192.168.0.103", 8888);

            //if (client.Connected)
            //{

            clientSocket.Send(code.GetBytes(record.Argument + delimeter));
            clientSocket.Send(code.GetBytes(record.EventTime.ToString("yyyy-MM-dd HH:mm:ss") + delimeter));

            //IFormatter formatter = new BinaryFormatter();
            //Stream s = new MemoryStream();
            //formatter.Serialize(s, record);
            //byte[] buffer = ((MemoryStream)s).ToArray();

            //byte[] doneAsBytes = Encoding.ASCII.GetBytes("*done*");
            //List<byte> queryAsBytes = new List<byte>();
            //queryAsBytes.AddRange(buffer);
            //queryAsBytes.AddRange(doneAsBytes);

            //    //stream.Write(queryAsBytes.ToArray(), 0, queryAsBytes.ToArray().Length);
            //    //stream.Flush();

            //    client.Client.Send(queryAsBytes.ToArray(), 0, queryAsBytes.ToArray().Length,SocketFlags.None);

            Console.WriteLine("query sent: " + record.Argument);
            //}
        }
    }
}
