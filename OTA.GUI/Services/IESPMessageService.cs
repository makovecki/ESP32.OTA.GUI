using OTA.GUI.Services.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OTA.GUI.Services
{
    public interface IESPMessageService
    {
        Task DiscoverESPAsync(int receiveTimeout = 100);
        void SaveDataAsync(ESP esp, string name);
        void AddListener(int messageType, Action<ESP> action);
        void StartOTAUpdate(string fileName, int port, ESP esp, string url);
    }
}
