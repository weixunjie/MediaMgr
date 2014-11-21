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

        void Application_Error(object sender, EventArgs e)
        {
            //捕获整个解决方案下的所有异常
            try
            {
                HttpUnhandledException eHttp = this.Server.GetLastError() as HttpUnhandledException;
                Exception eApp = eHttp.InnerException;
                GlobalUtils.AddConnectionTestLogs("系统异常", eApp.Message);
            }
            catch { }
        }


    }
}