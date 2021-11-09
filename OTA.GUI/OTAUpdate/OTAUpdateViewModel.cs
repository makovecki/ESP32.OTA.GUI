using OTA.GUI.FindESP.Model;
using OTA.GUI.Services;
using OTA.GUI.Services.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Core.Dispatcher;
using UI.Core.Navigation;
using UI.Core.Presentation;

namespace OTA.GUI.OTAUpdate
{
    public class OTAUpdateViewModel : Observable, IViewModel
    {
        private readonly IESPMessageService messageService;
        private readonly IHTTPServer httpsServer;
        private readonly ESP esp;
        private readonly IDispatcher dispatcher;
        public OTAUpdateViewModel(ESPEcho esp, IESPMessageService messageService, IHTTPServer httpsServer, IDispatcher dispatcher)
        {
            this.esp = esp;
            this.messageService = messageService;
            this.httpsServer = httpsServer;
            this.dispatcher = dispatcher;
            Name = "OTA update for " + esp.Name;
            FileName = OTA.Default.FilePath;
            BrowseCommand = Make.UICommand.Do(() => Browse());
            UpdateCommand = Make.UICommand.When(()=>!string.IsNullOrEmpty(FileName)).Do(() => Update());
            CancelCommand = Make.UICommand.Do(() => Cancel());
            messageService.AddListener(1, (message) => ProcessMessage(message));
            IsEnabled = true;
        }

        private void Cancel()
        {
            IsUpdating = false;
            httpsServer.Cancel();
        }

        private void ProcessMessage(ESP message)
        {
            dispatcher.BeginInvoke(()=> throw new Exception(System.Text.Encoding.Default.GetString(message.Buffer.Skip(3).ToArray())));
        }

        private async void Update()
        {
            IsUpdating = true;
            Percent = 0;
            if (FileName != null)
            {
                var taskServer =httpsServer.Start(FileName, (percent) => dispatcher.BeginInvoke(()=> Percent = percent));
                if (Rebuild)
                {
                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo() { UseShellExecute = true,   WorkingDirectory = Path.GetDirectoryName(FileName)?.Replace("build","") };
                    info.FileName = @"C:\Users\BorisMakovecki\.espressif\tools\idf-exe\1.0.1\idf.py.exe";
                    info.Arguments = "build";
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    
                    p.StartInfo = info;
                    
                    p.Start();
                    //p.WaitForExit();
                    //string output = p.StandardOutput.ReadToEnd();
                }
                messageService.StartOTAUpdate(Path.GetFileName(FileName), httpsServer.Port, esp,$"http://192.168.1.2:{httpsServer.Port}/");//{System.Net.Dns.GetHostName()}
                await taskServer;
                Cancel();

            }
        }

        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; OnPropertyChanged(); }
        }


        private bool isUpdating;

        public bool IsUpdating
        {
            get { return isUpdating; }
            set { isUpdating = value; OnPropertyChanged(); IsEnabled = !value; }
        }

        private void Browse()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".bin";
            dlg.Filter = "ESP bin image (.bin)|*.bin";

            if (dlg.ShowDialog() == true) FileName = dlg.FileName;
            
        }

        private string? fileName;

        public string? FileName
        {
            get { return fileName; }
            set { fileName = value; OnPropertyChanged(); Make.UICommand.Refresh(UpdateCommand); SaveFileName(); }
        }

        private void SaveFileName()
        {
            OTA.Default.FilePath = FileName;
            OTA.Default.Save();
        }

        public string Name { get; private set; }

        public bool IsSingleInstance => false;

        public ICommand BrowseCommand { get; set; }

        public bool Rebuild { get; set; }

        public ICommand UpdateCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private double percent;

        public double Percent
        {
            get { return percent; }
            set { percent = value; OnPropertyChanged(); }
        }

    }
}
