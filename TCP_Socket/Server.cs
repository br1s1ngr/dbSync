using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Server
{
    public static class Server
    {
        private static int Port { get; set; }
        static Socket serverSocket;
        static Encoding code = Encoding.ASCII;
        static string delimeter = "_end_";
        static string storage;
        static List<string> query_time;// = new string[2];
        static string query;
        static string hash;
        static string time;

        static TcpListener listener;

        public static void init(string[] serverConfig)
        {
            try
            {
                Port = int.Parse(serverConfig[0]);
                //listener = new TcpListener(IPAddress.Any, 8888);
                server_listen();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private static void server_listen()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            //serverSocket.Listen(-1);

            //listener = new TcpListener(IPAddress.Any, Port);
            //listener.Start(-1);
            //stream = new NetworkStream(listener.AcceptSocket());

        }

        public static int getPort()
        {
            return Port;
        }

        public static void Connect()
        {
            serverSocket.Listen(-1);
            //listener.Start();
            Console.WriteLine("Listening......");
            Console.WriteLine();

            do
            {
                //Socket socket = listener.AcceptSocket();
                Socket socket = serverSocket.Accept();

                while (true)
                {
                    //Socket handlerSocket = listener.AcceptSocket();
                    //                NetworkStream s = handlerSocket.Acc
                    //if (handlerSocket.Connected)
                    //{
                    //stream = new NetworkStream(socket);
                    try
                    {
                        acceptQuery(socket);
                        //Console.WriteLine("*****************");
                        //Thread thread = new Thread(new ThreadStart(acceptQuery));
                        //thread.Start();
                        //sendResponse();
                        
                        //socket.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                    //}
                    //}
                }
            } while (true);
        }

        private static void sendResponse(Socket socket)
        {
            //byte[] msg = Encoding.ASCII.GetBytes("recieved");
            ////handlerSocket.Send(msg);
            //stream.Write(msg, 0, msg.Length);
            //stream.Flush();
            //stream.Close();

            Console.WriteLine("sending response");
            socket.Send(code.GetBytes(ID + delimeter));
        }

        private static void acceptQuery(Socket socket)
        {
            //Socket handlerSocket = (Socket)alSockets[alSockets.Count - 1];
            //long x = stream.Length;
            //stream = new NetworkStream(listener.Server);

            query_time = new List<string>();
            int byteCount = 0;
            byte[] rgb = new byte[8192];

            while ((byteCount = socket.Receive(rgb)) > 0)
            {
                recieveBytes(rgb, byteCount);
                if (query_time.Count == 2) break;
            }

            //    thisRead = stream.Read(rgb, 0, blockSize);
            //    listBytes.AddRange(rgb.Take(thisRead));
            //    if (thisRead == 0)
            //        break;
            //}

            defineParameters();
            if (!DbConnect.DbConnect.QueryInLog(ID))
                runQuery();
            sendResponse(socket);
        }

        private static void defineParameters()
        {
            ID = int.Parse(query_time.ElementAt(0));
            string query = query_time.ElementAt(1);
        }

        private static bool runQuery()
        {
            try
            {
                //string hash = getQueryHash(query);
                DbConnect.DbConnect.RunQuery(query);
                //DbConnect.DbConnect.SaveTimeHashSuccess(time, hash);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //    }
        //}
        //dbconnect = new DbConnect.DbConnect(recievedQuery);
        //Thread insertQueryThread = new Thread(new ThreadStart(dbconnect.RunQuery));
        //insertQueryThread.Start();
        // stream.Close();
        //}

        private static void recieveBytes(byte[] rgb, int byteCount)
        {
            storage += code.GetString(rgb, 0, byteCount);
            int x;

            while ((x = storage.IndexOf(delimeter)) >= 0)
            {
                query_time.Add(storage.Substring(0, x));
                storage = storage.Substring(x + 5);
            }
        }

        private static string getQueryHash(string query)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(query));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }


        public static int ID { get; set; }
    }
}