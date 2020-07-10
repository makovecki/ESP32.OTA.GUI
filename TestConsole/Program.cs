using System;
using System.Net;
using System.Net.Sockets;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = new TcpClient();
            //IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[8];
            //IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 61163);
            //client.Connect(ipEndPoint);
            //Console.WriteLine("Hello World!");
            TcpClient tcpClient = new TcpClient();
            IPAddress ipAddress = Dns.GetHostEntry("www.google.com").AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 80);

            tcpClient.Connect(ipEndPoint);
        }
    }
}
