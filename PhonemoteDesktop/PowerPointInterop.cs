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
        private Dictionary<string, PowerPoint.Presentation> Presentations = new Dictionary<string, PowerPoint.Presentation>();

        //
        private (int, int, int) Counts = (0, 0, 0);

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
        public EventHandler OnUpdate;

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

            if (!isBusy) {
                Presentations.Clear();
                Counts = (PP_Ps.Count, PP_PVWs.Count, PP_SSWs.Count);

                int id = 0;
                foreach (PowerPoint.Presentation pre in PP_Ps)
                {
                    Presentations.Add($"{id}::PP::{pre.Name}", pre);
                    id++;
                }

                for (int i = 1; i <= PP_PVWs.Count; i++) 
                {
                    PowerPoint.Presentation pre = PP_PVWs[i].Presentation;
                    Presentations.Add($"{i}::PVW::{pre.Name}", pre);
                }
                
                
                for (int i = 1; i <= PP_SSWs.Count; i++)
                {
                    PowerPoint.Presentation pre = PP_SSWs[i].Presentation;
                    Presentations.Add($"{i}::SSW::{pre.Name}", pre);
                }
                
            }

            foreach ((string id, PowerPoint.Presentation pre) in Presentations)
            {
                Console.WriteLine("==========");

                Console.WriteLine($">> id: {id}");
                Console.WriteLine($">> name: {pre.Name}");

                //pre.SlideShowSettings.Run(); // Opedns the slideshow
                //pre.SlideShowWindow.View.Next();

                var fName = pre.FullName;
                Console.WriteLine($"fName: {fName}");

                /*
                try
                {

                    var n = pre.SlideShowWindow.GetType().GetRuntimeProperty("Active");

                    //pre.SlideShowSettings.Run();
                    //pre.SlideShowWindow.Activate();

                    var t = pre.SlideShowWindow.View;

                    //pre.SlideShowWindow.Active = OfficeOverrides.MsoTriState.msoTrue;

                    Console.WriteLine($">>>> TRUE {t.CurrentShowPosition}");
                } catch (Exception e)
                {
                    //Console.WriteLine($">>>> Exception: {e}");
                    Console.WriteLine($">>>> FALSE");
                }
                */

                // Recall C:\Windows\assembly\GAC_MSIL\office\15.0.0.0__71e9bce111e9429c
                //var p = pre.DisplayComments;

                //var p = pre.SlideShowWindow.Active;

                /*
                foreach (PropertyInfo prop in props)
                {
                    object propValue = prop.GetValue(pre.Application, null);

                    Console.WriteLine($">>>> prop: {propValue.ToString()}");
                    // Do something with propValue
                }
                */

                Console.WriteLine($">> active?: ");

                

                Console.WriteLine("==========");
            }

            OnUpdate.Invoke(this, EventArgs.Empty);
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
        private class PowerPointObject
        {
            private string id;
            private int currentSlide = 0;
            private bool hasOpenSlideShow = false;

            private void Init()
            {
                
            }
            public PowerPointObject()
            {
                
            }
        }
    }
}
