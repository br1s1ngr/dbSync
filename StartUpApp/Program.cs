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
            Console.WriteLine("Hello....Welcome");
            Console.WriteLine("Starting........");
            Console.WriteLine("***********************");

            Thread newThrd = new Thread(new ThreadStart(TCP_Server.Server.init));
            Thread newThrd2 = new Thread(new ThreadStart(TCP_Client.Client.init));
            //newThrd.Start();
            newThrd2.Start();

            //string uri = AppDomain.CurrentDomain.BaseDirectory;
            //string uri2 = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //uri2 = System.Reflection.Assembly.GetExecutingAssembly().EntryPoint.ToString();
            //string[] DatabaseConfig = System.IO.File.ReadAllLines(System.Net.Mime.MediaTypeNames.Application.StartupPath + "\\serverconfig1.cfg")
        }
    }
}
