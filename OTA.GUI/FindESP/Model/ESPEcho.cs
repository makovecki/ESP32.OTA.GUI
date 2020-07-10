using OTA.GUI.Services.Model;
using System.Net;
using System.Linq;

namespace OTA.GUI.FindESP.Model
{
    public class ESPEcho : ESP
    {
        public ESPEcho(byte[] buffer, IPAddress ip, int port) : base(buffer, ip, port)
        {
        }

        public string Name => System.Text.Encoding.Default.GetString(this.Buffer.Skip(3).ToArray());
    }
}
