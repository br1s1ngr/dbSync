using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCP_Server
{
    public static class Server
    {
        private static int Port { get; set; }

        private static TcpListener listener;

        public static void init()
        {
            initServerInfo();
            Connect();
        }


        public static int getPort()
        {
            return Port;
        }


        private static void initServerInfo()
        {
            try
            {
                string[] clientConfig = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\slaveDbConfig.cfg");
                Port = int.Parse(clientConfig[0]);
                //listener = new TcpListener(IPAddress.Any, 8888);
                listener = new TcpListener(IPAddress.Any, Port);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void Connect()
        {            
            listener.Start();
            Console.WriteLine("Listening......");
            while (true)
            {
                Socket handlerSocket = listener.AcceptSocket();
                if (handlerSocket.Connected)
                    acceptQuery(handlerSocket);
                //Console.WriteLine("*****************");
                //Thread thread = new Thread(new ThreadStart(acceptQuery));
                //thread.Start();
            }
        }

        private static void acceptQuery(Socket handlerSocket)
        {
            //Socket handlerSocket = (Socket)alSockets[alSockets.Count - 1];
            NetworkStream stream = new NetworkStream(handlerSocket);

            int thisRead = 0;
            int blockSize = 1024;
            List<byte> listBytes = new List<byte>();
            byte[] databyte = new byte[blockSize];
            while (true)
            {
                //handlerSocket.Receive(databyte);
                thisRead = stream.Read(databyte, 0, blockSize);
                listBytes.AddRange(databyte.AsEnumerable());
                if (thisRead == 0)
                    break;
            }
            //var xxx = bytes.ToArray(typeof(byte));
                string recievedQuery = Encoding.ASCII.GetString(listBytes.ToArray());

                handlerSocket = null;
                Console.WriteLine("query recieved: " + recievedQuery);
                Console.WriteLine();
                DbConnect.DbConnect dbConnectObj = new DbConnect.DbConnect();
                dbConnectObj.RunQuery(recievedQuery);
            //dbconnect = new DbConnect.DbConnect(recievedQuery);
                //Thread insertQueryThread = new Thread(new ThreadStart(dbconnect.RunQuery));
                //insertQueryThread.Start();

        }

        
    }
}
