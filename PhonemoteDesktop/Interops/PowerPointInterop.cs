using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerPoint = Microsoft.Office.Interop.PowerPoint; // This is the COM reference PowerPoint
using Microsoft.Office.Core;

namespace PhonemoteDesktop
{
    class PowerPointInterop
    {
        private Dictionary<string, PowerPoint.Presentation> OpenPowerPoints = new Dictionary<string, PowerPoint.Presentation>();

        public bool Loaded { get; }

        public int TotalPowerPoints { get; }

        public bool IsApplicationOpen { get; }

        private PowerPoint.Application PowerPointApplication = null;

        private System.Timers.Timer timer = null;

        private void GetAllOpenPresentations() 
        {
            OpenPowerPoints.Clear();

            // Get array of all normal presentations
            PowerPoint.Presentations PP_Ps = null;

            // Get array of all protected view presentations
            PowerPoint.ProtectedViewWindows PP_PVWs = null;

            // Get array of slide show presentations
            PowerPoint.SlideShowWindows PP_SSWs = null;

            bool isBusy = false;

            try {
                // Application might be busy

                PP_Ps   = PowerPointApplication.Presentations;
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
                Console.WriteLine("Report this exception");

                return;
            }

            if (isBusy)
            {
                return;
            }

            int id = 0;

            // Add all normal presentations to dictionary of all presentations
            foreach (PowerPoint.Presentation pre in PP_Ps) 
            {
                OpenPowerPoints.Add(pre.Name + ":" + id, pre);
                id++;
            }

            // Add all protected view presentations to dictionary of all presentations
            for (int i = 1; i <= PP_PVWs.Count; i++) 
            {
                PowerPoint.Presentation pre = PP_PVWs[i].Presentation;

                OpenPowerPoints.Add(pre.Name + ":" + id, pre);
                id++;
            }

            /*
            // Add all protected view presentations to dictionary of all presentations
            foreach (PowerPoint.Presentation pre in PP_PVWs)
            {
                OpenPowerPoints.Add(pre.Name + ":" + id, pre);
                id++;
            }
            // Add all slide show presentations to dictionary of all presentations
            foreach (PowerPoint.Presentation pre in PP_SSWs)
            {
                OpenPowerPoints.Add(pre.Name + ":" + id, pre);
                id++;
            }*/
        }
        private void UpdatePowerPointList(object source, System.Timers.ElapsedEventArgs e) 
        {

            GetAllOpenPresentations();

            foreach ((string name, PowerPoint.Presentation pre) in OpenPowerPoints) 
            {
                Console.WriteLine("PowerPoint: " + name + " found.");
            }
            Console.WriteLine();
        }

        public PowerPointInterop()
        {

            timer = new System.Timers.Timer();
            timer.Interval = 500;

            try
            {
                PowerPointApplication = new PowerPoint.Application();

                Loaded = true;

                Console.WriteLine("PowerPoint successfully loaded!");

                timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdatePowerPointList);
            }
            catch (Exception e)
            {
                Loaded = false;

                Console.WriteLine("Error loading PowerPoint!");
                Console.WriteLine(e);

                return;
            }

            timer.Start();
        }
    }
}
