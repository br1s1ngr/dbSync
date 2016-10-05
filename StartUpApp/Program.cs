using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StartUpApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello....Welcome");
            //Console.WriteLine("Starting........");
            //Console.WriteLine("***********************");

            initDB();
            initClient();
            //initServer();

            //Thread copyLogThread = new Thread(new ThreadStart(CopyLog));
            //copyLogThread.Start();

            //Thread newThrd = new Thread(new ThreadStart(TCP_Server.Server.Connect));
            Thread newThrd2 = new Thread(new ThreadStart(TCP_Client.Client.Begin));
            //newThrd.Start();
            newThrd2.Start();

            //string uri = AppDomain.CurrentDomain.BaseDirectory;
            //string uri2 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //uri2 = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.ToString();
            //string[] DatabaseConfig = System.IO.File.ReadAllLines(System.Net.Mime.MediaTypeNames.Application.StartupPath + "\\serverconfig1.cfg")
        }

        private static void initDB()
        {
            string[] connectionInfo = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\dbConfig.cfg");
            DbConnect.DbConnect.init(connectionInfo);
        }

        private static void initClient()
        {
            string[] clientConfig = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\masterDbConfig.cfg");
            TCP_Client.Client.init(clientConfig);
        }

        private static void initServer()
        {
            string[] serverconfig = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\slaveDbConfig.cfg");
            TCP_Server.Server.init(serverconfig);
        }

        private static void CopyLog()
        {
            while (true)
            {
                List<DbConnect.DbConnect.LogTableRecord> general_log_queries = DbConnect.DbConnect.GetQueriesFromGeneral();

                foreach (DbConnect.DbConnect.LogTableRecord item in general_log_queries)
                {
                    DbConnect.DbConnect.InsertQueryIntoClientLog(item);
                    DbConnect.DbConnect.UpdateGeneralLog(item);
                }
            }
        }

    }
}
