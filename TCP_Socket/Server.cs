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
        static NetworkStream stream;

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
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start(-1);
            //stream = new NetworkStream(listener.AcceptSocket());

        }

        public static int getPort()
        {
            return Port;
        }

        public static void Connect()
        {            
            listener.Start();
            Console.WriteLine("Listening......");
            Console.WriteLine();

            Socket socket = listener.AcceptSocket();

            while (true)
            {
                //Socket handlerSocket = listener.AcceptSocket();
//                NetworkStream s = handlerSocket.Acc
                //if (handlerSocket.Connected)
                //{

                if (socket.Connected)
                    stream = new NetworkStream(socket);
                try
                {
                    acceptQuery();
                    //Console.WriteLine("*****************");
                    //Thread thread = new Thread(new ThreadStart(acceptQuery));
                    //thread.Start();
                    //sendResponse();
                }
                catch (Exception ex)
                {

                }
                //}
            }
        }

        private static void sendResponse()
        {
            Console.WriteLine("sending response");
            byte[] msg = Encoding.ASCII.GetBytes("recieved");
            //handlerSocket.Send(msg);
            stream.Write(msg, 0, msg.Length);
            stream.Flush();
            //stream.Close();
        }

        private static void acceptQuery()
        {
            //Socket handlerSocket = (Socket)alSockets[alSockets.Count - 1];
            //long x = stream.Length;
            //stream = new NetworkStream(listener.Server);

            int thisRead = 0;
            int blockSize = 1024;
            List<byte> listBytes = new List<byte>();
            byte[] databyte = new byte[blockSize];

            while (stream.DataAvailable)
            {
                thisRead = stream.Read(databyte, 0, blockSize);
                listBytes.AddRange(databyte.Take(thisRead));
                if (thisRead == 0)
                    break;
            }
                string recievedQuery = Encoding.ASCII.GetString(listBytes.ToArray());
               
                if (!String.IsNullOrWhiteSpace(recievedQuery))
                {
                    Console.WriteLine("query recieved: " + recievedQuery);
                    Console.WriteLine();

                    if (DbConnect.DbConnect.RunQuery(recievedQuery))
                        sendResponse();
                }
            //dbconnect = new DbConnect.DbConnect(recievedQuery);
                //Thread insertQueryThread = new Thread(new ThreadStart(dbconnect.RunQuery));
                //insertQueryThread.Start();
                // stream.Close();
        }        
    }
}