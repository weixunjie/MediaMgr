using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using MediaMgrSystem.DataModels;

using System.Net;
using System.Text;
using System.Threading;
using Microsoft.AspNet.SignalR.Infrastructure;
using MediaMgrSystem.BusinessLayerLogic;
namespace MediaMgrSystem
{

    public static class SendLogic
    {

        public static void SendPlayCommand(string channelId, string channelName, string[] programeIds, IHubConnectionContext hub, string scheduleTaskGuidId)
        {

            lock (GlobalUtils.PublicObjectForLock)
            {

                bool isSchedule = !string.IsNullOrWhiteSpace(scheduleTaskGuidId);

                string errorrNotOpenVideoSvr = "视频服务器未开启";

                if (!isSchedule)
                {
                    //手工模式一次只能操作一个通道
                    if (GlobalUtils.IsChannelManuallyPlaying)
                    {
                        List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                        hub.Clients(alPCIds).sendResultBrowserClient("正在播放中,请先停止", "200");
                        return;
                    }

                    foreach (var sTask in GlobalUtils.RunningSchudules)
                    {
                        //优先手工播放
                        if (sTask.ChannelId == channelId)
                        {
                            SendStopRoRepeatCommand(channelId, hub, sTask.GuidId, channelId);

                        }
                    }


                }

                if (string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
                {

                    if (isSchedule)
                    {

                        if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                        {

                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = scheduleTaskGuidId;

                            cr.errorCode = "200";

                            cr.message = "视频服务器未开启";
                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                        }
                    }
                    else
                    {
                        List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                        hub.Clients(alPCIds).sendResultBrowserClient(errorrNotOpenVideoSvr, "200");
                        hub.Clients(alPCIds).sendResultBrowserClientNoticeStatus(errorrNotOpenVideoSvr, "200");
                    }


                    return;
                }

                else
                {
                    if (isSchedule)
                    {
                        foreach (var sTask in GlobalUtils.RunningSchudules)
                        {
                            //取消上个同个通道的计划
                            if (sTask.ChannelId == channelId)
                            {
                                SendStopRoRepeatCommand(channelId, hub, sTask.GuidId, channelId);

                            }
                        }
                    }

                    VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                    cmdToVideoSvr.commandType = CommandTypeEnum.PLAYVEDIO;

                    List<ProgramInfo> pids = GlobalUtils.ProgramBLLInstance.GetProgramByIds(programeIds);


                    cmdToVideoSvr.arg = new VideoServerOperArg();


                    cmdToVideoSvr.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");


                    GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();


                    cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;


                    cmdToVideoSvr.arg.streamName = "123456790" + channelId;

                    cmdToVideoSvr.arg.streamSrcs = new List<string>();

                    foreach (var pi in pids)
                    {
                        if (pi.MappingFiles != null && pi.MappingFiles.Count > 0)
                        {
                            foreach (var file in pi.MappingFiles)
                            {
                                cmdToVideoSvr.arg.streamSrcs.Add(file.FileName);
                            }
                        }
                    }

                    int intCID = 0;

                    int port = 0;
                    if (int.TryParse(channelId, out intCID))
                    {
                        port = 1001 + intCID;
                    }

                    cmdToVideoSvr.arg.udpBroadcastAddress = "udp://229.0.0.1:1234";// +port.ToString();
                    cmdToVideoSvr.arg.streamName = "123456790" + port.ToString();

                    List<string> clientsIpToSend = new List<string>();

                    List<string> clientsConectionIdToSend = new List<string>();

                    VideoOperAndriodClientCommand clientsDatraToSend = new VideoOperAndriodClientCommand();

                    CreateCommandForAndriodClients(pids, cmdToVideoSvr, channelId, out clientsIpToSend, out clientsConectionIdToSend, out clientsDatraToSend);

                    PushQueue("播放视频", clientsIpToSend);

                    GlobalUtils.VideoSvrArg = cmdToVideoSvr.arg;


                    TimeSpan beforeSendToAllClient = new TimeSpan(DateTime.Now.Ticks);


                    string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(clientsDatraToSend);
                    hub.Clients(clientsConectionIdToSend).sendMessageToClient(jsonDataToClient);

                    TimeSpan afterSendToAllClient = new TimeSpan(DateTime.Now.Ticks);

                    double offsetTotalMilliseconds = afterSendToAllClient.Subtract(beforeSendToAllClient).Duration().TotalMilliseconds;


                    int bufferTime = (int)(4000 - offsetTotalMilliseconds);

                    cmdToVideoSvr.arg.buffer = cmdToVideoSvr.arg.buffer = !string.IsNullOrWhiteSpace(scheduleTaskGuidId) ? bufferTime.ToString() : (bufferTime - 2000).ToString();

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
                    hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonData);

                    new Thread(ProcessTimeOutRequest).Start();

                    if (!isSchedule)
                    {
                        GlobalUtils.ChannelManuallyPlayingChannelId = channelId;

                        GlobalUtils.ChannelManuallyPlayingPids = programeIds;

                        GlobalUtils.ChannelManuallyPlayingChannelName = channelName;

                        GlobalUtils.IsChannelManuallyPlaying = true;
                    }
                }

            }


        }

        public static void SendStopRoRepeatCommand(string commandType, IHubConnectionContext hub, string scheduleTaskGuidId, string channelId)
        {

            bool isSchedule = !string.IsNullOrWhiteSpace(scheduleTaskGuidId);

            string errorrNotOpenVideoSvr = "视频服务器未开启";

            string videoSvrId = GlobalUtils.GetVideoServerConnectionIds();

            if (string.IsNullOrWhiteSpace(videoSvrId))
            {
                if (isSchedule)
                {
                    ComuResponseBase cr = new ComuResponseBase();

                    cr.guidId = scheduleTaskGuidId;

                    cr.errorCode = "200";

                    cr.message = "视频服务器未开启";

                    if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                    {

                        hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                    }

                }
                else
                {
                    List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                    hub.Clients(alPCIds).sendResultBrowserClient(errorrNotOpenVideoSvr, "200");
                    hub.Clients(alPCIds).sendResultBrowserClientNoticeStatus(errorrNotOpenVideoSvr, "200");
                }

            }

            else
            {
                SendOutStopRepeatCommandToServerAndClient(commandType, hub, channelId, isSchedule);
            }



        }

        private static void SendOutStopRepeatCommandToServerAndClient(string commandType, IHubConnectionContext hub,
            string channelId, bool isSchedule)
        {
            VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

            cmdToVideoSvr.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


            GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();

            cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;


            cmdToVideoSvr.arg = new VideoServerOperArg();

            cmdToVideoSvr.arg.streamName = "1234567890" + channelId;

            List<string> clientsIpToSend = new List<string>();

            List<string> clientsConectionIdToSend = new List<string>();

            VideoOperAndriodClientCommand clientsDatraToSend = new VideoOperAndriodClientCommand();

            CreateCommandForAndriodClients(null, cmdToVideoSvr, channelId, out clientsIpToSend, out clientsConectionIdToSend, out clientsDatraToSend);


            clientsDatraToSend.arg = new VideoOperAndriodClientArg();
            clientsDatraToSend.arg.streamName = "1234567890" + channelId;

            PushQueue(commandType == "1" ? "停止" : "循环播放", clientsIpToSend);

            string jsonDataToVideoSvr = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
            hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonDataToVideoSvr);


            string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(clientsDatraToSend);

            hub.Clients(clientsIpToSend).sendMessageToClient(jsonDataToClient);

            new Thread(ProcessTimeOutRequest).Start();

            if (!isSchedule)
            {
                GlobalUtils.ChannelManuallyPlayingChannelId = string.Empty;
                GlobalUtils.ChannelManuallyPlayingPids = null;
                GlobalUtils.ChannelManuallyPlayingChannelName = string.Empty;
                GlobalUtils.IsChannelManuallyPlaying = false;
            }
        }

        private static void ProcessTimeOutRequest()
        {
            Thread.Sleep(2000);


            lock (GlobalUtils.PublicObjectForLock)
            {
                List<QueueItem> queueToRemoved = new List<QueueItem>();
                foreach (var que in GlobalUtils.CommandQueues)
                {
                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);

                    TimeSpan tsSubmited = new TimeSpan(que.PushTicks);
                    if (ts.Subtract(tsSubmited).Duration().TotalMilliseconds >= 3000)
                    {
                        queueToRemoved.Add(que);

                        //Write timeout logs hereeeeeeeeeeeeeee.
                    }
                }

                if (queueToRemoved != null && queueToRemoved.Count > 0)
                {
                    foreach (QueueItem item in queueToRemoved)
                    {
                        GlobalUtils.CommandQueues.Remove(item);
                        System.Diagnostics.Debug.WriteLine("Remove Command No Response: Now count is :" + GlobalUtils.CommandQueues.Count);
                    }

                }
            }
        }


        private static VideoOperAndriodClientCommand CreateAndroidCommandForStopRepeat(string commandType)
        {
            VideoOperAndriodClientCommand cmdToAndroidClient = new VideoOperAndriodClientCommand();

            cmdToAndroidClient.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


            GlobalUtils.CurrentClientGuidId = Guid.NewGuid().ToString();

            cmdToAndroidClient.guidId = GlobalUtils.CurrentClientGuidId;
            return cmdToAndroidClient;
        }


        private static void PushQueue(string cmdText, List<string> clientIps)
        {
            long currentTicks = DateTime.Now.Ticks;

            GlobalUtils.CommandQueues.Add(new QueueItem() { PushTicks = currentTicks, IpAddressStr = GlobalUtils.VideoServerIPAddress, GuidIdStr = GlobalUtils.CurrentVideoGuidId, CommandStr = cmdText });

            foreach (var ip in clientIps)
            {
                GlobalUtils.CommandQueues.Add(new QueueItem() { PushTicks = currentTicks, IpAddressStr = ip, GuidIdStr = GlobalUtils.CurrentClientGuidId, CommandStr = cmdText });
            }
        }


        private static void CreateCommandForAndriodClients(List<ProgramInfo> pids, VideoServerOperCommand cmdToVideoSvr, string channelId,
            out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out VideoOperAndriodClientCommand dataToSend)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = CommandTypeEnum.PLAYVEDIO;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();
            GlobalUtils.CurrentClientGuidId = dataSendToAndroidClient.guidId;


            dataSendToAndroidClient.arg = new VideoOperAndriodClientArg();


            if (pids != null && pids.Count > 0)
            {
                dataSendToAndroidClient.arg.bitRate = pids[0].MappingFiles[0].BitRate;

                dataSendToAndroidClient.arg.mediaType = GlobalUtils.CheckIfAudio(pids[0].MappingFiles[0].FileName) ? 1 : 2;

                dataSendToAndroidClient.arg.streamSrcs = cmdToVideoSvr.arg.streamSrcs;

                dataSendToAndroidClient.arg.udpBroadcastAddress = cmdToVideoSvr.arg.udpBroadcastAddress;
            }

            dataToSend = dataSendToAndroidClient;

            List<GroupInfo> channelGroups = GlobalUtils.GroupBLLInstance.GetGroupByChannelId(channelId);

            List<string> needSentClientIpAddresses = new List<string>();

            if (channelGroups != null && channelGroups.Count > 0)
            {
                foreach (var gi in channelGroups)
                {
                    if (gi.Devices != null && gi.Devices.Count > 0)
                    {
                        foreach (var di in gi.Devices)
                        {
                            needSentClientIpAddresses.Add(di.DeviceIpAddress);
                        }
                    }
                }
            }

            if (needSentClientIpAddresses.Count > 0)
            {
                ipsNeedToSend = needSentClientIpAddresses;
                idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses);
            }
            else
            {
                ipsNeedToSend = new List<string>();
                idsNeedToSend = new List<string>();
            }
        }


        //public static void SendPlayCommand(string channelId, string[] programeIds, IHubConnectionContext hub,bool isScheduled)
        //{
        //    string errorrNotOpenVideoSvr = "视频服务器未开启";
        //    if (GlobalUtils.IsChannelPlaying && !isSendToSvr)
        //    {
        //        List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
        //        hub.Clients(alPCIds).sendResultBrowserClient("正在播放中,请先停止", "200");
        //        return;
        //    }


        //    if (string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId) && !isSendToSvr)
        //    {
        //        List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
        //        hub.Clients(alPCIds).sendResultBrowserClient(errorrNotOpenVideoSvr, "200");
        //        hub.Clients(alPCIds).sendResultBrowserClientNoticeStatus(errorrNotOpenVideoSvr, "200");
        //        return;
        //    }
        //    else
        //    {

        //        VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

        //        cmdToVideoSvr.commandType = CommandTypeEnum.PLAYVEDIO;


        //        List<ProgramInfo> pids = GlobalUtils.ProgramBLLInstance.GetProgramByIds(programeIds);


        //        cmdToVideoSvr.arg = new VideoServerOperArg();


        //        cmdToVideoSvr.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");


        //        GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();


        //        cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;
        //        cmdToVideoSvr.arg.buffer = "0";
        //        cmdToVideoSvr.arg.streamName = "123456790";




        //        cmdToVideoSvr.arg.streamSrcs = new List<string>();

        //        foreach (var pi in pids)
        //        {
        //            if (pi.MappingFiles != null && pi.MappingFiles.Count > 0)
        //            {
        //                foreach (var file in pi.MappingFiles)
        //                {
        //                    cmdToVideoSvr.arg.streamSrcs.Add(file.FileName);
        //                }
        //            }
        //        }

        //        int intCID = 0;

        //        int port = 0;
        //        if (int.TryParse(channelId, out intCID))
        //        {
        //            port = 1001 + intCID;
        //        }

        //        cmdToVideoSvr.arg.udpBroadcastAddress = "udp://229.0.0.1:1234";// +port.ToString();
        //        cmdToVideoSvr.arg.streamName = "123456790" + port.ToString();

        //        CreatePlayCommandForAndriodClients(pids, cmdToVideoSvr, channelId);

        //        PushQueue("播放视频");

        //        GlobalUtils.VideoSvrArg = cmdToVideoSvr.arg;

        //        if (!isSendToSvr)
        //        {
        //            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
        //            GlobalUtils.ReadyToSentSVRData = jsonData;
        //        }
        //       // string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);

        //        if (isSendToSvr)
        //        {
        //            hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(GlobalUtils.ReadyToSentSVRData);
        //        }             



        //        string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(GlobalUtils.ReadyToSentClientData);


        //        if (!isSendToSvr)
        //        {             
        //            hub.Clients(GlobalUtils.ReadyToSentClientIds).sendMessageToClient(jsonDataToClient);
        //        }

        //      //  

        //        GlobalUtils.IsChannelPlaying = true;
        //    }


        //}


    }
}