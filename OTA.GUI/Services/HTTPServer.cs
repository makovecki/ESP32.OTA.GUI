using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OTA.GUI.Services
{
    public class HTTPServer : IHTTPServer
    {
        public HTTPServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://+:{Port}/");
        }
        private HttpListener? listener;
        public int Port => 1973;
        //netsh http add sslcert ipport = 0.0.0.0:1973 certhash = 219b3b86e7048b0bea47e915b887d2fd9c2bb8fa appid = { AB2F65CE - 045E-4807 - 81EE - 129F59235F07 }
        //netsh http add urlacl url = https://+:1973/ user=AzureAD\BorisMakovecki
        //netsh http delete urlacl url=https://+:1973/
        //netsh http add urlacl url = http://+:1973/ user=AzureAD\BorisMakovecki
        private bool IsCanceled { get; set; }

        public Task Start(string binFile, Action<double> progress)
        {
            IsCanceled = false;
            listener?.Start();

            return Task.Run(async () =>
            {
                var context = listener?.GetContext();
                context.Response.SendChunked = false;
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                context.Response.AddHeader("Content-disposition", "attachment; filename=" + Path.GetFileName(binFile));
                using (var stream = context.Response.OutputStream)
                {
                    using (FileStream fs = File.OpenRead(binFile))
                    {
                        context.Response.ContentLength64 = fs.Length;
                        byte[] buffer = new byte[64 * 1024];
                        int read;
                        int readed = 0;
                        using (BinaryWriter bw = new BinaryWriter(stream))
                        {
                            try
                            {
                                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    if (!IsCanceled)
                                    {
                                        bw.Write(buffer, 0, read);
                                        bw.Flush(); //seems to have no effect
                                        readed += read;
                                        progress(100.0 * readed / fs.Length);

                                    }
                                }

                                bw.Close();
                            }
                            catch (Exception ex)
                            { 
                            }
                        }

                    }
                    await Task.Delay(500);// wait for ESP to write to SPI
                    if (!IsCanceled)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.StatusDescription = "OK";
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        context.Response.StatusDescription = "Canceled";
                    }
                    stream.Close();
                }
            });
        }

        public void Stop()
        {
            listener?.Stop();
        }

        public void Cancel()
        {
            IsCanceled = true;
            Stop();
        }
    }
}
