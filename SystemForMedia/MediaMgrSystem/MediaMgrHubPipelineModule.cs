using MediaMgrSystem.DataModels;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MediaMgrSystem
{


    public class MediaMgrHubPipelineModule : HubPipelineModule
    {

        public MediaMgrHubPipelineModule()
        {

           

        }


        protected override void OnAfterConnect(IHub hub)
        {


            SingalConnectedClient sc = new SingalConnectedClient();
            SingalRClientConnectionType singalRClientConnectionType = SingalRClientConnectionType.PC;
            sc.ConnectionId = hub.Context.ConnectionId;

            string type = string.Empty;

            string strIdentify = string.Empty;



            if (hub.Context.QueryString["clientType"] != null)
            {
                type = hub.Context.QueryString["clientType"].ToString().ToUpper();

                if (type == "ANDROID")
                {

                    singalRClientConnectionType = SingalRClientConnectionType.ANDROID;
                }
                else if (type == "VIDEOSERVER")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.VEDIOSERVER;

                    hub.Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendResultBrowserClient("视频服务器连接成功", "20");

                }

                else if (type == "ENCODER")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.ENCODERDEVICE;
                }

                else if (type == "WINDOWSSERVICE")
                {
                    singalRClientConnectionType = SingalRClientConnectionType.WINDOWSSERVICE;

                }
            }

            if (hub.Context.QueryString["clientIdentify"] != null)
            {
                strIdentify = hub.Context.QueryString["clientIdentify"].ToString();
            }

            sc.ConnectionType = singalRClientConnectionType;



            if (singalRClientConnectionType != SingalRClientConnectionType.ANDROID)
            {
                strIdentify = Guid.NewGuid().ToString();

            }
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

                GlobalUtils.AddLogs(hub.Clients, "系统提示", "视频服务器已连接");


            }

            if (sc.ConnectionType == SingalRClientConnectionType.WINDOWSSERVICE)
            {

                string extingId = GlobalUtils.GetWindowsServiceConnectionIds();

                if (!string.IsNullOrEmpty(extingId))
                {
                    GlobalUtils.RemoveConnectionByConnectionId(extingId);

                }

                GlobalUtils.AddLogs(hub.Clients, "系统提示", "后台计划服务已连接");

                GlobalUtils.WindowsServiceConnectionId = strIdentify;
                GlobalUtils.WindowsServiceConnectionId = hub.Context.ConnectionId;


            }

            if (sc.ConnectionType == SingalRClientConnectionType.ANDROID)
            {


                List<DeviceInfo> dis = GlobalUtils.DeviceBLLInstance.GetADevicesByIPAddress(sc.ConnectionIdentify);

                if (dis != null && dis.Count > 0)
                {

                }
                else
                {
                    DeviceInfo di = new DeviceInfo();
                    di.DeviceIpAddress = sc.ConnectionIdentify;
                    di.DeviceName = sc.ConnectionIdentify;
                    di.GroupId = "-1";
                    di.UsedToAudioBroandcast = true;

                    di.UsedToVideoOnline = false;

                    GlobalUtils.DeviceBLLInstance.AddDevice(di);

                }

                SendRefreshNotice(hub);

            }

            GlobalUtils.AddConnection(sc);

            string str="Someone Connected: Connected Id" + hub.Context.ConnectionId;
            System.Diagnostics.Debug.WriteLine(str);

            GlobalUtils.WriteDebugLogs(str);


        }

        private void SendRefreshNotice(IHub hub)
        {

            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            hub.Clients.Clients(ids).sendRefreshDeviceMessge();
        }

        protected override void OnAfterDisconnect(IHub hub)
        {
            DateTime dt = DateTime.UtcNow;

            string str="DISConnected: Connected Id" + hub.Context.ConnectionId;
            System.Diagnostics.Debug.WriteLine(str);

            GlobalUtils.WriteDebugLogs(str);
            if (hub.Context.ConnectionId == GlobalUtils.VideoServerConnectionId)
            {
                GlobalUtils.VideoServerIPAddress = string.Empty; ;
                GlobalUtils.VideoServerConnectionId = string.Empty;

                GlobalUtils.AddLogs(hub.Clients, "系统异常", "视频服务器断开连接");

            }

            if (hub.Context.ConnectionId == GlobalUtils.WindowsServiceConnectionId)
            {
                GlobalUtils.WindowsServiceConnectionId = string.Empty;

                GlobalUtils.AddLogs(hub.Clients, "系统异常", "后台计划服务断开连接");

            }

            if (GlobalUtils.CheckIfConnectionIdIsAndriod(hub.Context.ConnectionId))
            {
                SendRefreshNotice(hub);
            }

            GlobalUtils.RemoveConnectionByConnectionId(hub.Context.ConnectionId);

        }

        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            return base.OnAfterIncoming(result, context);
        }

    }
}

