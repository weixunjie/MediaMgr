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


namespace MediaMgrSystem
{
    public class Global : HttpApplication
    {
        private object lockObjet = new object();
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalHost.HubPipeline.AddModule(new MediaMgrHubPipelineModule());

            GlobalHost.Configuration.ConnectionTimeout = new TimeSpan(0, 120, 0);
            GlobalHost.Configuration.DisconnectTimeout = new TimeSpan(0, 0, 6);

        }

        //    System.Timers.Timer timer = new System.Timers.Timer(60 * 60 * 1000);


        //    timer.AutoReset = true;

        //    timer.Enabled = true;

        //    timer.Elapsed += timer_Elapsed;


        //}

        //void timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    lock (GlobalUtils.PublicObjectForLockClientMsg)
        //    {
        //        IHubConnectionContext allClients = GlobalHost.ConnectionManager.GetHubContext("MediaMgrHub").Clients;
        //        allClients.All.sendKeepAlive("KeepAlive");
        //    }

        //}

      



    }
}