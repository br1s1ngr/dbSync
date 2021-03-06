﻿using System;
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
        static int queryId;

        public static int getPort()
        {
            return Port;
        }

        public static string getIP()
        {
            return IP;
        }

        // = new TcpClient();
        private static List<DbConnect.DbConnect.ClientLogTableRecord> queries;

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
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(IP, Port);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("*****error: " + ex.Message);
#endif
            }
        }

        public static void Begin()
        {
#if DEBUG
            Console.WriteLine("Connecting......");
            Console.WriteLine();
#endif
            while (true)
            {
                queries = DbConnect.DbConnect.GetQueriesFromClientLog();
                if (queries.Count > 0)
                {
                    if (clientSocket.Connected)
                        BeginSendingQueries();
                    else
                    {
                        Thread.Sleep(1000);
                        connectClient();
                    }
                }
                Thread.Sleep(50);
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
                    queryId = -1;
                    sendQuery(record);
                    if (recieveResponse())
                    {
                        DbConnect.DbConnect.UpdateRecordInClientLog(queryId);
#if DEBUG
                        Console.WriteLine("log updated for query number: " + queryId);
#endif
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
                    Console.WriteLine();
#endif  
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
            {
                recieveBytes(rgb, byteCount);
                if (storage == delimeter)
                    return true;
               
            }
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
            int x;

            while ((x = storage.IndexOf(delimeter)) > 0)
            {
                string temp = storage.Substring(0, x);
                queryId = int.Parse(temp);
                //storage = storage.Remove(0, x);
                storage = storage.Substring(x);
            }
        }

        private static void updateLog(DbConnect.DbConnect.ClientLogTableRecord logRecord)
        {
            DbConnect.DbConnect.UpdateRecordInClientLog(logRecord.ID);
#if DEBUG
            Console.WriteLine("query updated");
            Console.WriteLine("****************************");
#endif
        }

        private static void sendQuery(DbConnect.DbConnect.ClientLogTableRecord record)
        {
            //TcpClient client = new TcpClient(IP, Port);
            //TcpClient client = new TcpClient("192.168.0.103", 8888);

            //if (client.Connected)
            //{

            clientSocket.Send(code.GetBytes(record.ID/*.ToString()*/ + delimeter));
            clientSocket.Send(code.GetBytes(record.Query + delimeter));
            //clientSocket.Shutdown(SocketShutdown.Send);
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
#if DEBUG
            Console.WriteLine("query sent: " + record.Query);
#endif
            //}
        }
    }
}
