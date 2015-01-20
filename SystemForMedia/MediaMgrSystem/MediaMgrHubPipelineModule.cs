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

            GlobalUtils.SingalConnectedClientsBLLIntance.RemoveAll();

        }

        //protected override void OnAfterReconnect(IHub hub)
        //{
        //    System.Diagnostics.Debug.WriteLine("Reconnectining->" + hub.Context.ConnectionId);
        //    lock (GlobalUtils.PublicObjectForLockConnected)
        //    {


        //        SingalConnectedClient sc = new SingalConnectedClient();
        //        SingalRClientConnectionType singalRClientConnectionType = SingalRClientConnectionType.PC;
        //        sc.ConnectionId = hub.Context.ConnectionId;

        //        string type = string.Empty;

        //        string strIdentify = string.Empty;


        //        if (hub.Context.QueryString["clientIdentify"] != null)
        //        {
        //            strIdentify = hub.Context.QueryString["clientIdentify"].ToString();
        //        }

        //        if (hub.Context.QueryString["clientType"] != null)
        //        {
        //            type = hub.Context.QueryString["clientType"].ToString().ToUpper();

        //            if (type == "ANDROID")
        //            {

        //                singalRClientConnectionType = SingalRClientConnectionType.ANDROID;



        //            }
        //            else if (type == "VIDEOSERVER")
        //            {
        //                singalRClientConnectionType = SingalRClientConnectionType.VEDIOSERVER;
        //                return;


        //            }

        //            else if (type == "ENCODERFORAUDIO")
        //            {

        //                singalRClientConnectionType = SingalRClientConnectionType.ENCODERAUDIODEVICE;


        //            }

        //            else if (type == "WINDOWSSERVICE")
        //            {

        //                singalRClientConnectionType = SingalRClientConnectionType.WINDOWSSERVICE;
        //                return;

        //            }
        //            else if (type == "REMOTECONTORLDEVICE")
        //            {
        //                singalRClientConnectionType = SingalRClientConnectionType.REMOTECONTORLDEVICE;

        //            }

        //        }

        //        sc.ConnectionType = singalRClientConnectionType;

        //        sc.ConnectionId = hub.Context.ConnectionId;

        //        sc.ConnectionIdentify = strIdentify;
        //        string extingCid = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(strIdentify, sc.ConnectionType.ToString());

        //        if (!string.IsNullOrEmpty(extingCid))
        //        {
        //            GlobalUtils.DeleteSingalConnectedClientByConnectIds(extingCid);
        //        }

        //        if (sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE)
        //        {
        //            SendRefreshRemoteControlDeviceNotice(hub);
        //        }

        //        if (sc.ConnectionType == SingalRClientConnectionType.ANDROID)
        //        {
        //            SendRefreshAudioDeviceNotice(hub);
        //        }

        //        if (!string.IsNullOrEmpty(sc.ConnectionId) && !string.IsNullOrEmpty(sc.ConnectionIdentify))
        //        {


        //            GlobalUtils.AddConnection(sc);
        //        }
        //    }

        //}

        protected override void OnAfterConnect(IHub hub)
        {
            System.Diagnostics.Debug.WriteLine("Connected->" + hub.Context.ConnectionId);
            ProcessConnect(hub);
        }

        private void ProcessConnect(IHub hub)
        {
            lock (GlobalUtils.PublicObjectForLockConnected)
            {

                SingalConnectedClient sc = new SingalConnectedClient();
                SingalRClientConnectionType singalRClientConnectionType = SingalRClientConnectionType.PC;
                sc.ConnectionId = hub.Context.ConnectionId;

                string type = string.Empty;

                string strIdentify = string.Empty;


                if (hub.Context.QueryString["clientIdentify"] != null)
                {
                    strIdentify = hub.Context.QueryString["clientIdentify"].ToString();
                }

                String macAddress = string.Empty;
                if (hub.Context.QueryString["macAddress"] != null)
                {
                    macAddress = hub.Context.QueryString["macAddress"];

                }

                if (hub.Context.QueryString["clientType"] != null)
                {
                    type = hub.Context.QueryString["clientType"].ToString().ToUpper();

                    if (type == "ANDROID")
                    {

                        singalRClientConnectionType = SingalRClientConnectionType.ANDROID;

                        UpgradeInfo ui = GlobalUtils.UpgradeConfigBLLInstance.GetUpgradeConfig("1");

                        SendSyncTimeAndVersionCheck(ui, hub, singalRClientConnectionType);


                    }
                    else if (type == "VIDEOSERVER")
                    {
                        singalRClientConnectionType = SingalRClientConnectionType.VEDIOSERVER;

                        hub.Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendResultBrowserClient("视频服务器连接成功", "20");

                    }

                    else if (type == "ENCODERFORAUDIO")
                    {

                        singalRClientConnectionType = SingalRClientConnectionType.ENCODERAUDIODEVICE;

                        EncoderAudioSendGroupsInfoCommand cmdSyncGrouOps = new EncoderAudioSendGroupsInfoCommand();
                        cmdSyncGrouOps.commandType = CommandTypeEnum.ENCODERSENDGROUPSINFO;
                        cmdSyncGrouOps.guidId = Guid.NewGuid().ToString();
                        cmdSyncGrouOps.groups = new List<EncoderSyncGroupInfo>();

                        EncoderAudioInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(strIdentify);

                        if (ei == null)
                        {
                            EncoderAudioInfo eif = new EncoderAudioInfo();
                            eif.BaudRate = "1100";
                            eif.ClientIdentify = strIdentify;
                            eif.EncoderName = strIdentify;
                            eif.Priority = "1";
                            GlobalUtils.EncoderBLLInstance.AddEncoder(eif);

                        }
                        //  GlobalUtils.VideoEncoderQueues
                        List<GroupInfo> gi = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(BusinessType.AUDITBROADCAST);

                        foreach (var g in gi)
                        {

                            if (g.Devices != null && g.Devices.Count > 0)
                            {
                                cmdSyncGrouOps.groups.Add(new EncoderSyncGroupInfo { groupId = g.GroupId, groupName = g.GroupName });
                            }



                        }

                        hub.Clients.Client(hub.Context.ConnectionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmdSyncGrouOps));



                        EncoderAudioSendDevsInfoCommand cmdSyncDev = new EncoderAudioSendDevsInfoCommand();
                        cmdSyncDev.commandType = CommandTypeEnum.ENCODERSENDDEVINFO;
                        cmdSyncDev.guidId = Guid.NewGuid().ToString();
                        cmdSyncDev.devs = new List<EncoderSyncDevInfo>();


                        foreach (var g in gi)
                        {
                            if (g.Devices != null && g.Devices.Count > 0)
                            {
                                foreach (var di in g.Devices)
                                {
                                    cmdSyncDev.devs.Add(new EncoderSyncDevInfo { devId = di.DeviceId, devName = di.DeviceName });
                                }
                            }
                        }

                        hub.Clients.Client(hub.Context.ConnectionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmdSyncDev));


                    }

                    else if (type == "WINDOWSSERVICE")
                    {
                        singalRClientConnectionType = SingalRClientConnectionType.WINDOWSSERVICE;

                    }
                    else if (type == "REMOTECONTORLDEVICE")
                    {
                        singalRClientConnectionType = SingalRClientConnectionType.REMOTECONTORLDEVICE;

                        UpgradeInfo ui = GlobalUtils.UpgradeConfigBLLInstance.GetUpgradeConfig("2");

                        SendSyncTimeAndVersionCheck(ui, hub, singalRClientConnectionType);
                    }

                }



                sc.ConnectionType = singalRClientConnectionType;



                if (singalRClientConnectionType == SingalRClientConnectionType.PC)
                {
                    strIdentify = Guid.NewGuid().ToString();

                }
                sc.ConnectionIdentify = strIdentify;

                if (sc.ConnectionType == SingalRClientConnectionType.VEDIOSERVER)
                {

                    string extingViedoServerCid = GlobalUtils.VideoServerConnectionId;

                    if (!string.IsNullOrEmpty(extingViedoServerCid))
                    {
                        GlobalUtils.DeleteSingalConnectedClientByConnectIds(extingViedoServerCid);

                    }

                    GlobalUtils.AddLogs(hub.Clients, "系统提示", "视频服务器已连接");


                }

                if (sc.ConnectionType == SingalRClientConnectionType.WINDOWSSERVICE)
                {

                    string extingId = GlobalUtils.GetWindowsServiceConnectionIds();

                    if (!string.IsNullOrEmpty(extingId))
                    {
                        GlobalUtils.DeleteSingalConnectedClientByConnectIds(extingId);

                    }

                    GlobalUtils.AddLogs(hub.Clients, "系统提示", "后台计划服务已连接");



                }

                if (sc.ConnectionType == SingalRClientConnectionType.ANDROID ||
                    sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE)
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
                        di.MacAddress = macAddress;
                        di.UsedToAudioBroandcast = true;
                        di.UsedToVideoOnline = false;

                        di.UsedToRemoteControl = sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE;

                        int re = GlobalUtils.DeviceBLLInstance.AddDevice(di);

                        if (re < 0)
                        {
                            if (re == -10)
                            {
                                GlobalUtils.AddLogs(hub.Clients, "系统提示", "音频终端已经达到最大数量");
                            }
                            else if (re == -11)
                            {

                                GlobalUtils.AddLogs(hub.Clients, "系统提示", "视频终端已经达到最大数量");
                            }
                            else if (re == -12)
                            {
                                GlobalUtils.AddLogs(hub.Clients, "系统提示", "物联终端已经达到最大数量");
                            }


                            di.UsedToAudioBroandcast = false;
                            GlobalUtils.DeviceBLLInstance.AddDevice(di);
                        }


                    }


                    string extingCid = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(strIdentify, sc.ConnectionType.ToString());

                    if (!string.IsNullOrEmpty(extingCid))
                    {
                        GlobalUtils.DeleteSingalConnectedClientByConnectIds(extingCid);
                    }

                    if (sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE)
                    {
                        SendRefreshRemoteControlDeviceNotice(hub);
                    }

                    if (sc.ConnectionType == SingalRClientConnectionType.ANDROID)
                    {
                        SendRefreshAudioDeviceNotice(hub);
                    }



                }

                if (sc.ConnectionType == SingalRClientConnectionType.ENCODERAUDIODEVICE)
                {
                    SendRefreshCallerEncoderDeviceMessge(hub);
                }


                GlobalUtils.AddConnection(sc);

                string str = "Someone Connected: Connected Id" + hub.Context.ConnectionId;
                System.Diagnostics.Debug.WriteLine(str + "Date Time->" + DateTime.Now.ToString("HH:mm:ss"));

                if (sc.ConnectionType != SingalRClientConnectionType.PC)
                {
                    GlobalUtils.AddConnectionTestLogs(sc.ConnectionType.ToString(), "设备：(" + sc.ConnectionIdentify + ")已连接");
                }


                GlobalUtils.WriteDebugLogs(str);

                if (sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE)
                {
                    ComunicationBase cmd = new ComunicationBase();

                    cmd.guidId = Guid.NewGuid().ToString();
                    cmd.commandType = CommandTypeEnum.REMOTECONTRL_COMMAND_REQUEST_STATE;

                    hub.Clients.Client(hub.Context.ConnectionId).sendRemoteControlToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));
                }


            }
        }

        private void SendSyncTimeAndVersionCheck(UpgradeInfo ui, IHub hub, SingalRClientConnectionType type)
        {

            SyncTimeCommand cmd = new SyncTimeCommand();

            cmd.guidId = Guid.NewGuid().ToString();
            cmd.commandType = CommandTypeEnum.SYNCTIME;
            cmd.arg = new SyncTimeCommandArg();

            cmd.arg.upgradeUrl = ui.UpgardeUrl;
            cmd.arg.upgradeVer = String.IsNullOrEmpty(ui.VersionId) ? "1" : ui.VersionId;
            cmd.arg.serverNowTime = ((DateTime.UtcNow.Ticks - 621355968000000000) / 10000).ToString();
            if (type == SingalRClientConnectionType.ANDROID)
            {
                hub.Clients.Client(hub.Context.ConnectionId).sendMessageToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));
            }
            else if (type == SingalRClientConnectionType.REMOTECONTORLDEVICE)
            {
                hub.Clients.Client(hub.Context.ConnectionId).sendRemoteControlToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));

            }
        }
        private void SendRefreshAudioDeviceNotice(IHub hub)
        {

            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            hub.Clients.Clients(ids).sendRefreshAudioDeviceMessge();
        }

        private void SendRefreshCallerEncoderDeviceMessge(IHub hub)
        {

            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            hub.Clients.Clients(ids).sendRefreshCallerEncoderDeviceMessge();
        }


        private void SendRefreshRemoteControlDeviceNotice(IHub hub)
        {

            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            hub.Clients.Clients(ids).sendRefreshRemoteControlDeviceMessge();
        }


        protected override void OnAfterDisconnect(IHub hub)
        {

            DateTime dt = DateTime.UtcNow;

            SingalRClientConnectionType disConnectType = SingalRClientConnectionType.PC;

            string str = "DISConnected: Connected Id" + hub.Context.ConnectionId;
            System.Diagnostics.Debug.WriteLine(str);

            GlobalUtils.WriteDebugLogs(str);
            if (hub.Context.ConnectionId == GlobalUtils.VideoServerConnectionId)
            {
                disConnectType = SingalRClientConnectionType.VEDIOSERVER;
                SendRefreshAudioDeviceNotice(hub);
                GlobalUtils.AddLogs(hub.Clients, "系统异常", "视频服务器断开连接");
                GlobalUtils.AddConnectionTestLogs("系统异常", "视频服务器断开连接");

            }

            if (hub.Context.ConnectionId == GlobalUtils.WindowsServiceConnectionId)
            {
                disConnectType = SingalRClientConnectionType.WINDOWSSERVICE;
                SendRefreshAudioDeviceNotice(hub);
                GlobalUtils.AddLogs(hub.Clients, "系统异常", "后台计划服务断开连接");
                GlobalUtils.AddConnectionTestLogs("系统异常", "后台计划服务断开连接");
            }

            if (GlobalUtils.CheckIfConnectionIdIsAndriod(hub.Context.ConnectionId))
            {
                disConnectType = SingalRClientConnectionType.ANDROID;
                SendRefreshAudioDeviceNotice(hub);


            }

            if (GlobalUtils.CheckIfConnectionIdIsAudioEncoder(hub.Context.ConnectionId))
            {
                disConnectType = SingalRClientConnectionType.ENCODERAUDIODEVICE;
                SendRefreshAudioDeviceNotice(hub);


            }

            if (GlobalUtils.CheckIfConnectionIdIsRemoteControlDevice(hub.Context.ConnectionId))
            {
                disConnectType = SingalRClientConnectionType.REMOTECONTORLDEVICE;
                SendRefreshRemoteControlDeviceNotice(hub);
            }


            if (GlobalUtils.CheckIfConnectionIdIsAudioEncoder(hub.Context.ConnectionId))
            {
                SendRefreshCallerEncoderDeviceMessge(hub);
            }


            String ci = GlobalUtils.GetIdentifyByConectionId(hub.Context.ConnectionId);


            if (disConnectType == SingalRClientConnectionType.ENCODERAUDIODEVICE)
            {

                object[] objes = new object[2];
                objes[0] = ci;
                objes[1] = hub.Clients;
                new Thread(ProcesStopLogs).Start(objes);
            }


            if (disConnectType != SingalRClientConnectionType.PC)
            {



                GlobalUtils.AddConnectionTestLogs(disConnectType.ToString(), "设备：(" + ci + ")断开连接");
            }


            GlobalUtils.DeleteSingalConnectedClientByConnectIds(hub.Context.ConnectionId);



        }
        private void ProcesStopLogs(object cbObj)
        {
            object[] ojbs = cbObj as object[];

            string ci = ojbs[0].ToString();

            IHubCallerConnectionContext clients = (IHubCallerConnectionContext)ojbs[1];

            if (!string.IsNullOrEmpty(ci))
            {
                RunningEncoder re = GlobalUtils.EncoderAudioRunningClientsBLLInstance.CheckIfEncoderRunning(ci);
                if (re != null && !string.IsNullOrEmpty(re.ClientIdentify))
                {

                    CallerEncoderControlLogic.SendEncoderAudioCloseCommand(clients, ci);
                }
            }
        }

        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            return base.OnAfterIncoming(result, context);
        }

    }
}

