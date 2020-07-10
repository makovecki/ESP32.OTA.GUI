using OTA.GUI.FindESP.Model;
using OTA.GUI.Services.Model;
using OTA.GUI.Services.TCPListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OTA.GUI.Services
{
    public class ESPMessageService : IESPMessageService
    {
        private readonly UdpClient client;
        
        private Byte[] buffer = new Byte[256];
        private readonly Dictionary<int, List<Action<ESP>>> listeners = new Dictionary<int, List<Action<ESP>>>();
        public ESPMessageService()
        {
            client = new UdpClient(0);
            ListenClientsAsync();
        }

        private void ListenClientsAsync()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var r = await client.ReceiveAsync();
                    var esp = ResolveESPMessageType(r.Buffer, r.RemoteEndPoint.Address, r.RemoteEndPoint.Port);
                    
                    if (listeners.TryGetValue(esp.MessageType, out List<Action<ESP>>? messageListeners)) messageListeners.ForEach(listener => listener(esp));
                }
            });
        }

        private ESP ResolveESPMessageType(byte[] buffer, IPAddress ip, int port)
        {
            switch (buffer[0])
            {
                case 0: return new ESPEcho(buffer, ip, port);
                default:
                    return new ESP(buffer, ip, port);
            }
        }

        public Task DiscoverESPAsync(int receiveTimeout = 100)
        {

            return Task.Factory.StartNew(() =>
            {
                var requestData = new byte[] { 0, 0, 2 }.Concat(BitConverter.GetBytes(((IPEndPoint)client.Client.LocalEndPoint).Port).Take(2).Reverse()).ToArray();
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress netip in host.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork))
                {
                    var ip = netip.GetAddressBytes();
                    ip[3] = 255;
                    client.Send(requestData, requestData.Length, new IPEndPoint(new IPAddress(ip), 1977));
                }
                Thread.Sleep(receiveTimeout);
                
            });
        }

        public void SaveDataAsync(ESP esp, string name)
        {
            var message = new byte[] { 2 }.Concat(BitConverter.GetBytes((Int16)name.Length).Reverse()).Concat(Encoding.ASCII.GetBytes(name)).ToArray();
            //client.Send(message, message.Length, new IPEndPoint(esp.Ip, 1973));
        }

        public void AddListener(int messageType, Action<ESP> action)
        {
            if (listeners.TryGetValue(messageType, out List<Action<ESP>>? messageListeners)) messageListeners.Add(action);
            else listeners.Add(messageType, new List<Action<ESP>> { action });
        }

        public void StartOTAUpdate(string fileName, int port, ESP esp, string url)
        {
            var requestData = new byte[] { 1 }.Concat(BitConverter.GetBytes((UInt16)url.Length).Reverse()).Concat(Encoding.ASCII.GetBytes(url)).ToArray();
                
            client.Send(requestData, requestData.Length, new IPEndPoint(esp.Ip, esp.Port));
        }

        
    }
}
