using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpWebview;
using SharpWebview.Content;

namespace PhonemoteDesktop
{
    class WebViewContainer
    {
        // Webview properties
        private const bool debugMode = true;
        private const int width = 800;
        private const int height = 800;
        private const string defaultTitle = "Loading...";

        //
        private readonly Webview wv = null;

        //
        private readonly Dictionary<string, HtmlContent> pageHTML = new Dictionary<string, HtmlContent>();

        //
        private string title = defaultTitle;
        public WebViewContainer(string defaultNavigate, string initScript = "")
        {
            // Generate Pages
            string[] pages = Properties.Resources.pageList.Split(',');
            foreach (string page in pages)
            {
                pageHTML.Add(page, new HtmlContent(Properties.Resources.ResourceManager.GetString(page)));
            }

            // Create Webview
            wv = new Webview(debugMode, true);
            wv.SetTitle(defaultTitle);
            wv.SetSize(width, height, WebviewHint.Fixed);

            //
            wv.Navigate(pageHTML[defaultNavigate]);

            //
            wv.InitScript(initScript);
        }
        public void AddBind(string name, Action<string, string> callback) 
        {
            wv.Bind(name, callback);
        }
        public void Run()
        {
            wv.Run();
        }
        public void Navigate(string location)
        {
            if (pageHTML.ContainsKey(location))
            {
                wv.Navigate(pageHTML[location]);
            }
        }
        public void Return(string id, RPCResult result, string resultJSON)
        {
            wv.Return(id, result, resultJSON);
        }
        public void ExecuteOnThread(Action callback)
        {
            wv.Dispatch(callback);
        }

        public string Title
        {
            get 
            {
                return title;
            }

            set
            {
                title = value;
                wv.SetTitle(value);
            }
        }
    }
}
