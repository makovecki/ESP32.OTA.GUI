using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OTA.GUI.Services.Model
{
    public class ESP
    {
        public ESP(byte[] buffer, IPAddress ip, int port )
        {
            Buffer = buffer;
            Ip = ip;
            Port = port;
            /*Buffer = echo.Buffer;
            Ip = echo.RemoteEndPoint.Address;
            Port = echo.RemoteEndPoint.Port;
            */
        }

        public byte[] Buffer { get; private set; }
        public IPAddress Ip { get; private set; }
        public int Port { get; private set; }
        public int MessageType => Buffer[0];
    }
}
