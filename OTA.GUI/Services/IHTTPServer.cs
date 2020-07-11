using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OTA.GUI.Services
{
    public interface IHTTPServer
    {
        int Port { get; }

        Task Start(string binFile, Action<double> progress);
        void Stop();
        void Cancel();
    }
}
