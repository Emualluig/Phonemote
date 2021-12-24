using System;
using System.Collections.Generic;
using SharpWebview;
using SharpWebview.Content;

namespace PhonemoteDesktop
{
    class Program
    {
        class Command
        {
            public string presentation
            {
                get;
                set;
            }
            public string command
            {
                get;
                set;
            }
        }
        [STAThread]
        static void Main(string[] args)
        {
            Server server = new Server();
            string location = server.location;

            string powerpointInteropJSON = "{}";
            //
            PowerPointInterop PPI = new PowerPointInterop();
            PPI.OnUpdate += (s, args) =>
            {
                server.MessageAll(args.JSON);
                powerpointInteropJSON = args.JSON;
            };
            //
            if (PPI.Loaded)
            {
                
            }

            // REWRITE POWERPOINT INTEROP TO USE EVENTS
            
            WebViewContainer webview = new WebViewContainer("initialize_page", $"window.qrURL = \"{location}\";");
            webview.AddBind("cs_extern_initialize", (id, f_args) => {
                string title = f_args.Trim(new char[] { '[', '\"', ']' });

                webview.Title = title;
            });
            webview.AddBind("cs_extern_get_ppi_json", (id, f_args) =>
            {
                webview.Return(id, RPCResult.Success, $"{{ result: {powerpointInteropJSON} }}");
            });

            server.OnOpen += (s, args) =>
            {
                Console.WriteLine("CONNECTION HERE");
                webview.ExecuteOnThread(() => {
                    webview.Navigate("index");
                });
            };
            server.OnMessage += (s, message) =>
            {
                // Handle commands
                Console.WriteLine($"{message}");

                Command t = Newtonsoft.Json.JsonConvert.DeserializeObject<Command>(message);

                string command = t.command;

                switch (command)
                {
                    case "next":
                        PPI.PowerPointCommandHandler($"{t.presentation}", PowerPointInterop.PowerPointCommands.Next);

                        break;
                    case "previous":
                        PPI.PowerPointCommandHandler($"{t.presentation}", PowerPointInterop.PowerPointCommands.Previous);

                        break;
                    default:
                        Console.WriteLine($"Unknown command {command}");
                        break;
                }
            };

            webview.Run();

        }
    }
}
