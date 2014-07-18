﻿using System;
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


            System.Timers.Timer timer = new System.Timers.Timer(60 * 60 * 1000);


            timer.AutoReset = true;

            timer.Enabled = true;

            timer.Elapsed += timer_Elapsed;


        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (GlobalUtils.PublicObjectForLockClientMsg)
            {
                IHubConnectionContext allClients = GlobalHost.ConnectionManager.GetHubContext("MediaMgrHub").Clients;
                allClients.All.sendKeepAlive("KeepAlive");
            }

        }

        void Application_Error(object sender, EventArgs e)
        {
            //获取到HttpUnhandledException异常，这个异常包含一个实际出现的异常
            Exception ex = Server.GetLastError();
            //实际发生的异常
            Exception iex = ex.InnerException;

            string errorMsg = String.Empty;
            string particular = String.Empty;
            if (iex != null)
            {
                errorMsg = iex.Message;
                particular = iex.StackTrace;
            }
            else
            {
                errorMsg = ex.Message;
                particular = ex.StackTrace;
            }

            GlobalUtils.AddLogs(null, "程序异常:", errorMsg);

        }



    }
}