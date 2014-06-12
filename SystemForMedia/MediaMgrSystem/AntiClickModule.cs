using MediaMgrSystem.DataModels;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediaMgrSystem
{


    public class AntiClickModule : HubPipelineModule
    {


        long weee = DateTime.Now.Ticks;
        public AntiClickModule()
        {
            Interval = 1000;
        }

        public int Interval { get; set; }

        private readonly ConcurrentDictionary<string, DateTime> _connections = new ConcurrentDictionary<string, DateTime>();

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        protected override void OnAfterConnect(IHub hub)
        {
            DateTime dt = DateTime.UtcNow;


            SingalConnectedClient sc = new SingalConnectedClient();
            SingalRClientConnectionType singalRClientConnectionType = SingalRClientConnectionType.PC;
            sc.ConnectionId = hub.Context.ConnectionId;

            string type = string.Empty;

            string strIdentify = string.Empty;
            if (hub.Context.QueryString["clientType"] != null)
            {
                type = hub.Context.QueryString["clientType"].ToString().ToUpper();

                if (type == "ANDRIOD")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.ANDROID;
                }
                else if (type == "VIDEOSERVER")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.VEDIOSERVER;
                }

                else if (type == "ENCODER")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.ENCODERDEVICE;
                }
            }

            if (hub.Context.QueryString["clientIdentify"] != null)
            {
                strIdentify = hub.Context.QueryString["clientIdentify"].ToString();
            }

            sc.ConnectionType = singalRClientConnectionType;

            sc.ConnectionIdentify = strIdentify;

            if (sc.ConnectionType == SingalRClientConnectionType.VEDIOSERVER)
            {

                string extingViedoServerCid = GlobalUtils.GetVideoServerConnectionIds();

                if (!string.IsNullOrEmpty(extingViedoServerCid))
                {
                    GlobalUtils.RemoveConnectionByConnectionId(extingViedoServerCid);

                }

                GlobalUtils.VideoServerIPAddress = strIdentify;
                GlobalUtils.VideoServerConnectionId = hub.Context.ConnectionId;


            }


            GlobalUtils.AddConnection(sc);

            System.Diagnostics.Debug.WriteLine("Someone Connected: Connected Id" + hub.Context.ConnectionId);




            //String message = "SYNCTIME{0}";
            //TimeSpan ts = new TimeSpan(dt.Ticks);
            //message = string.Format(message, (long)((dt - Jan1st1970).TotalMilliseconds));
            //hub.Clients.Client(hub.Context.ConnectionId).sendSyncMessage(message);
        }

        protected override void OnAfterDisconnect(IHub hub)
        {
            DateTime dt = DateTime.UtcNow;

            System.Diagnostics.Debug.WriteLine("DISConnected: Connected Id" + hub.Context.ConnectionId);

            if (hub.Context.ConnectionId == GlobalUtils.VideoServerConnectionId)
            {
                GlobalUtils.VideoServerIPAddress = string.Empty; ;
                GlobalUtils.VideoServerConnectionId = string.Empty;
            }

            GlobalUtils.RemoveConnectionByConnectionId(hub.Context.ConnectionId);

        }
        protected override void OnAfterOutgoing(IHubOutgoingInvokerContext context)
        {
            TimeSpan a = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan b = new TimeSpan(weee);
            //   double aa = a.Subtract(b).Duration().TotalMilliseconds;
            System.Diagnostics.Debug.WriteLine("after send:" + (a.TotalMilliseconds - b.TotalMilliseconds).ToString());
        }

        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {

            weee = DateTime.Now.Ticks;
            TimeSpan b = new TimeSpan(weee);
            System.Diagnostics.Debug.WriteLine("Before send" + b.TotalMilliseconds.ToString());
            return base.OnBeforeOutgoing(context);
        }
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            if (context.Args != null && context.Args.Count > 0 && context.Args[0].ToString() == "PC")
            {
                HttpContext.Current.Application["connectionId"] = context.Hub.Context.ConnectionId;
            }

            return true;
        }

        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            return base.OnAfterIncoming(result, context);
        }

    }
}

