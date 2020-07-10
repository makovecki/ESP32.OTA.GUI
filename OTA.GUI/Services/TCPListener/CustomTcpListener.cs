using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OTA.GUI.Services.TCPListener
{
    public class CustomTcpListener : TcpListener
    {
       
        public CustomTcpListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public new bool Active => base.Active;

    }
}
