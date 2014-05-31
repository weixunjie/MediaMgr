using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.AspNet.SignalR;

using System.Timers;
using Microsoft.AspNet.SignalR.Hubs;
using MediaMgrSystem.DataModels;
using System.IO;

namespace MediaMgrSystem
{
    public class Global : HttpApplication
    {
        private object lockObjet = new object();
        void Application_Start(object sender, EventArgs e)
        {
            // RouteTable.Routes.MapHubs();

            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalHost.HubPipeline.AddModule(new AntiClickModule());

            GlobalHost.Configuration.DisconnectTimeout = new TimeSpan(0,0,6);

            GlobalHost.Configuration.ConnectionTimeout = new TimeSpan(0, 0,1200);


             
            //System.Timers.Timer timer = new System.Timers.Timer(1000);


            //timer.AutoReset = true;

            //timer.Enabled = true;

            //timer.Elapsed += new System.Timers.ElapsedEventHandler(this.setTime);

        }

        

        public void setTime(Object sender, ElapsedEventArgs e)
        {
            
            lock (lockObjet)
            {
                IHubConnectionContext a = GlobalHost.ConnectionManager.GetHubContext("Test").Clients;

                if (GlobalUtils.andiordClients != null)
                {
                    //foreach (SingalConnectedClient sc in GlobalUtils.andiordClients)
                    //{
                    //    System.Diagnostics.Debug.WriteLine(sc.ConnectionId + "-" + sc.ConnectionIdentify + '-' + sc.ConnectionType.ToString());
                    //}
                }

                string lineStr = string.Empty;
                StreamReader sr = new StreamReader(@"c:\schedule.txt");

                lineStr = sr.ReadLine();

                sr.Close();

                string[] str = lineStr.Split(',');

                string dtNow = DateTime.Now.ToString("HH:mm:ss");


                List<string> result = new List<string>();
                if (Application["LastRunningMins"] != null)
                {
                    result = (List<string>)Application["LastRunningMins"];
                }

                for (int i = 0; i < str.Length; i++)
                {
                    TimeSpan tes = DateTime.Parse(dtNow).Subtract(DateTime.Parse(str[i])).Duration();
                    if (tes.TotalSeconds <= 5)
                    {
                        if (!result.Contains(str[i]))
                        {
                            Application.Lock();
                            result.Add(str[i]);
                            Application["LastRunningMins"] = result;

                            string exTime = DateTime.Now.ToString("HH:mm:ss");
                            System.Diagnostics.Debug.WriteLine("Schedule Execute At:" + exTime + "  Config Time:" + str[i]);

                            Class2.SetCommand(1, a);
                       
                            Application.UnLock();
                            return;
                        }
                    }

                }

            }
        }

    }
}