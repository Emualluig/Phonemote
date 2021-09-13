using System;
using System.Collections.Generic;
using SharpWebview;
using SharpWebview.Content;

namespace PhonemoteDesktop
{
    class Program
    {


        private static readonly bool debugMode = true;

        private static readonly int width = 800;
        private static readonly int height = 800;

        static void Main(string[] args)
        {
            PowerPointInterop powerPointInterop = new PowerPointInterop();

            if (!powerPointInterop.Loaded) {
                Console.WriteLine("POWERPOINT NOT DETECTED");
            }

            // Try not to use many pages
            Dictionary<string, HtmlContent> pageHTML = GeneratePageList();

            // This is an infinite loop
            using (var webview = new Webview(debugMode, true))
            {
                webview
                    .SetTitle("Loading...")
                    .SetSize(width, height, WebviewHint.Fixed)
                    .Bind("cs_extern_initialize", (id, f_args) => {
                        string title = f_args.Trim(new char[] { '[', '\"', ']' });

                        webview.SetTitle(title);
                    })
                    .Bind("cs_extern_navigate", (id, f_args) => {
                        string navTarget = f_args.Trim(new char[] { '[', '\"', ']' });

                        Console.WriteLine("Navigating to => " + navTarget);
                        webview.Navigate(pageHTML[navTarget]);
                    })
                    .Bind("cs_extern_send", (id, f_args) => {
                        Console.WriteLine("cs_send" + f_args);

                        webview.Return(id, RPCResult.Success, "{ result: " + f_args + " }");
                    })

                    // Go to index.html first
                    .Navigate(pageHTML["index"])
                    .Run();
            }
            
        }


        // GeneratePageList() returns a Dictionary<string, HtmlContent> where the key is the file name of the page
        //  and the value is the html
        // time: O(n) where n is the number of pages
        private static Dictionary<string, HtmlContent> GeneratePageList()
        {
            // get resource names
            string[] pages = Properties.Resources.pageList.Split(',');

            Dictionary<string, HtmlContent> pageContent = new Dictionary<string, HtmlContent>();

            foreach (string page in pages)
            {
                pageContent.Add(page, new HtmlContent(Properties.Resources.ResourceManager.GetString(page)));
            }

            return pageContent;
        }
    }
}
