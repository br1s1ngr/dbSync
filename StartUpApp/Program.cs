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

            TCP_Server.Server server = new TCP_Server.Server();

            Thread newThrd = new Thread(new ThreadStart(server.Connect));
            newThrd.Start();
        }
    }
}
