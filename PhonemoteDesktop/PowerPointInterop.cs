using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerPoint = Microsoft.Office.Interop.PowerPoint; // this is a COM reference

namespace PhonemoteDesktop
{
    class PowerPointInterop
    {
        //
        private PowerPoint.Application PowerPointApplication = null;

        //
        private Dictionary<string, PowerPointObject> Presentations = new Dictionary<string, PowerPointObject>();

        //
        private bool _Loaded = false;
        public bool Loaded { get { return _Loaded; } }
        private bool Initialize() 
        {
            try
            {
                PowerPointApplication = new PowerPoint.Application();
                
                _Loaded = true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                _Loaded = false;
                return false;
            }
        }

        //
        public EventHandler OnError;

        //
        private void Error(object source, EventArgs args)
        {
            OnError.Invoke(this, EventArgs.Empty);
        }

        //
        public class UpdateArguments : EventArgs
        {
            public bool isBusy { get; set; }
            public string JSON { get; set; }
        }

        //
        public EventHandler<UpdateArguments> OnUpdate;

        //
        private void Update(object source, System.Timers.ElapsedEventArgs args)
        {

            // Prepare to get presentations
            PowerPoint.Presentations PP_Ps = null;
            PowerPoint.ProtectedViewWindows PP_PVWs = null;
            PowerPoint.SlideShowWindows PP_SSWs = null;

            bool isBusy = false;

            // Get presentations, set isBusy to true if the application was busy
            try
            {
                PP_Ps = PowerPointApplication.Presentations;
                PP_PVWs = PowerPointApplication.ProtectedViewWindows;
                PP_SSWs = PowerPointApplication.SlideShowWindows;
            }
            catch (System.Runtime.InteropServices.COMException comExcep)
            {
                Console.WriteLine("Powerpoint Application is Busy");
                isBusy = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // If the program is not busy, create dictionary of PowerPointObject's
            if (!isBusy) {
                Presentations.Clear();

                foreach (PowerPoint.Presentation pp_pre in PP_Ps)
                {
                    PowerPointObject pre = new PowerPointObject(pp_pre);
                    Presentations.Add($"{pre.Name}", pre);
                }

                for (int i = 1; i <= PP_PVWs.Count; i++) 
                {
                    PowerPointObject pre = new PowerPointObject(PP_PVWs[i].Presentation);
                    Presentations.Add($"{pre.Name}", pre);
                }

#if false
    // This code is used to look up open slide shows, but it is now handled by the PowerPointObject instead

                for (int i = 1; i <= PP_SSWs.Count; i++)
                {
                    PowerPoint.Presentation pre = PP_SSWs[i].Presentation;
                    Presentations.Add($"{i}::SSW::{pre.Name}", pre);
                }
#endif
            }


            UpdateArguments UA = new UpdateArguments();
            UA.isBusy = isBusy;
            UA.JSON = JSONbuilder(isBusy);
            OnUpdate.Invoke(this, UA);
        }

        //
        private string JSONbuilder(bool isBusy)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append($"{{\"isBusy\":{isBusy.ToString().ToLower()},\"presentations\":");

            string presentationString = Newtonsoft.Json.JsonConvert.SerializeObject(Presentations.Select(keyValuePair => keyValuePair.Value)).ToString();
            sb.Append(presentationString);

            sb.Append($"}}");

            string s = sb.ToString();
            //Console.WriteLine($"{s}");

            return s;
        }

        // 
        private const int TIMER_INTERVAL = 500;
        private System.Timers.Timer timer = null;
        public PowerPointInterop()
        {
            if (Initialize())
            {
                timer = new System.Timers.Timer();
                timer.Interval = TIMER_INTERVAL;

                timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);

                timer.Start();
            }   
        }
        public enum PowerPointCommands
        {
            Next,
            Previous,
            SlideShow,
            Activate,
            ExitSlideShow
        }
        public void PowerPointCommandHandler(string name, PowerPointCommands command)
        {
            if (Presentations.ContainsKey(name))
            {
                PowerPointObject PPO = Presentations[name];

                switch (command)
                {
                    case PowerPointCommands.Next:
                        PPO.SlideNext();
                        break;
                    case PowerPointCommands.Previous:
                        PPO.SlidePrevious();
                        break;
                    case PowerPointCommands.SlideShow:
                        PPO.ShowSlideShow();
                        break;
                    case PowerPointCommands.Activate:
                        PPO.ShowSlideShow();
                        break;
                    case PowerPointCommands.ExitSlideShow:
                        PPO.ExitSlideShow();
                        break;
                }
            }
        }

        private class PowerPointObject
        {
            private string _id;
            private string _name;
            private readonly PowerPoint.Presentation presentation = null;

            private bool hasOpenSlideShow = false;
            private int currentSlide = 0;
            private readonly PowerPoint.SlideShowWindow slideShowWindow = null;
            private readonly PowerPoint.SlideShowView slideShowView = null;

            public PowerPointObject(PowerPoint.Presentation pre)
            {
                presentation = pre;

                _id = presentation.FullName;
                _name = presentation.Name;

                bool openSlideShow = true;
                try
                {
                    slideShowWindow = presentation.SlideShowWindow;
                }
                catch (System.Runtime.InteropServices.COMException comExcep)
                {
                    openSlideShow = false;
                }

                if (openSlideShow)
                {
                    hasOpenSlideShow = openSlideShow;
                    slideShowView = slideShowWindow.View;
                    currentSlide = slideShowView.CurrentShowPosition;

                    //Console.WriteLine($"{_name} has an open presentation, is on the {currentSlide} slide");
                }
                else
                {
                    //Console.WriteLine($"{_name} does not have an open presentation");
                }
            }

            public void SlideMoveTo(int slideNumber)
            {
                if (hasOpenSlideShow) 
                {
                    slideShowView.GotoSlide(slideNumber);
                    currentSlide = slideShowView.CurrentShowPosition;
                }
            }
            public void SlideNext()
            {
                if (hasOpenSlideShow)
                {
                    slideShowView.Next();
                    currentSlide = slideShowView.CurrentShowPosition;
                }
            }
            public void SlidePrevious()
            {
                if (hasOpenSlideShow)
                {
                    slideShowView.Previous();
                    currentSlide = slideShowView.CurrentShowPosition;
                }
            }
            public void ShowSlideShow()
            {
                if (hasOpenSlideShow)
                {
                    // Bring to foreground
                    slideShowWindow.Activate();
                }
                else 
                {
                    // Open slideshow and bring to foreground
                    presentation.SlideShowSettings.Run();
                    hasOpenSlideShow = true;
                }
            }
            public void ExitSlideShow()
            {
                if (hasOpenSlideShow) 
                {
                    slideShowView.Exit();
                }
            }

            public string Name
            {
                get
                {
                    return _name;
                }
            }
            public bool HasSlideShowOpen
            {
                get
                {
                    return hasOpenSlideShow;
                }
            }
            public int CurrentSlide
            {
                get 
                {
                    return currentSlide;
                }
            }
        }
    }
}
