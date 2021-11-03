using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Fleck;

namespace PhonemoteDesktop
{
    class Server
    {
        private string GetLocalIP {
            get {
                IPAddress[] hostAddresses = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (var address in hostAddresses) {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) 
                    {
                        return address.ToString();
                    }
                }

                return "ERROR";
            }
        }
        public string location { get; }
        public bool Error { get; }
        private string _ErrorMessage = "";

        public string GetErrorMessage() 
        {
            if (Error)
            {
                return _ErrorMessage;
            }
            else 
            {
                return "";
            }
        }
        public Server(string port = "8181")
        {
            Error = false;
         
            string ip = GetLocalIP.Split("%")[0];
            string full_ip = $"ws://[{ip}]:{port}";
            location = full_ip;

            var server = new WebSocketServer(full_ip);
            server.Start(socket =>
            {
                socket.OnOpen = () => Console.WriteLine("Open!");
                socket.OnClose = () => Console.WriteLine("Close!");
                socket.OnMessage = (message) => 
                {
                    Console.WriteLine($"Message: {message}");
                };
                socket.OnError = (error) =>
                {
                    Console.WriteLine($"Error: {error}");
                };
            });            
        }
    }
}
