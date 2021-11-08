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
        private readonly WebSocketServer server = null;
        private string GetLocalIPv6 {
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
        private string getLocalIPv4
        {
            get
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }

                return "ERROR";
            }
        }
        public string location { get; }
#if false
        public EventHandler OnOpen;
        private void Open(IWebSocketConnection socket)
        {
            OnOpen.Invoke(this, EventArgs.Empty);
        }

        public EventHandler OnClose;
        private void Close(IWebSocketConnection socket)
        {
            OnClose.Invoke(this, EventArgs.Empty);
        }

        public EventHandler OnMessage;
        private void Message(IWebSocketConnection socket, string message)
        {
            OnMessage.Invoke(this, EventArgs.Empty);
        }

        public EventHandler OnError;
        private void Error(IWebSocketConnection socket, Exception error)
        {
            OnError.Invoke(this, EventArgs.Empty);
        }

        public void Send()
        {
            server.
        }
#endif

        //
        public EventHandler OnOpen;
        private void Open(IWebSocketConnection socket)
        {
            OnOpen.Invoke(this, EventArgs.Empty);
        }

        //
        private List<IWebSocketConnection> SocketArray = new List<IWebSocketConnection>();
        public void MessageAll(string message)
        {
            foreach (IWebSocketConnection socket in SocketArray)
            {
                socket.Send(message);
            }
        }

        public EventHandler<string> OnMessage;
        private void Message(IWebSocketConnection socket, string message)
        {
            OnMessage.Invoke(this, message);
        }
        public Server(string port = "8181")
        {
#if false
            // AFTER LOTS OF TESTING, IPv6 DOES NOT WORK
            string ip = GetLocalIPv6.Split("%")[0];
            string full_ip = $"ws://[{ip}]:{port}";
            location = full_ip;
#endif
            string IPv4 = $"ws://{getLocalIPv4}:8181";
            location = IPv4;

            server = new WebSocketServer(IPv4, false);

            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    SocketArray.Add(socket);
                    Open(socket);
                };
                socket.OnClose = () =>
                {
                    SocketArray.RemoveAll(x => x == socket);
                };
                socket.OnMessage = (message) => 
                {
                    Message(socket, message);
                };
                socket.OnError = (error) =>
                {
                    
                };
            });            
        }
    }
}
