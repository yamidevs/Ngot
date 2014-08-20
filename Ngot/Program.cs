using Ngot.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ngot
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress Ip = IPAddress.Parse("127.0.0.1");

            IPEndPoint LocalEndPoint = new IPEndPoint(Ip, 444);
            TCPServer app = new TCPServer(LocalEndPoint, 100);
            app.Run();

            Console.ReadLine();
        }
    }
}
