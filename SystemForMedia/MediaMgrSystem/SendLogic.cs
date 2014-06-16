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


        public static void SendPlayCommand(string channelId, string[] programeIds, IHubConnectionContext hub,bool isScheduled)
        {
            string errorrNotOpenVideoSvr = "视频服务器未开启";
            if (GlobalUtils.IsChannelPlaying)
            {
                List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                hub.Clients(alPCIds).sendResultBrowserClient("正在播放中,请先停止", "200");
                return;
            }


            if (string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
            {
                List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                hub.Clients(alPCIds).sendResultBrowserClient(errorrNotOpenVideoSvr, "200");
                hub.Clients(alPCIds).sendResultBrowserClientNoticeStatus(errorrNotOpenVideoSvr, "200");
                return;
            }
            else
            {

                VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                cmdToVideoSvr.commandType = CommandTypeEnum.PLAYVEDIO;


                List<ProgramInfo> pids = GlobalUtils.ProgramBLLInstance.GetProgramByIds(programeIds);


                cmdToVideoSvr.arg = new VideoServerOperArg();


                cmdToVideoSvr.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");


                GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();


                cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;
             
                cmdToVideoSvr.arg.streamName = "123456790";

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

                CreatePlayCommandForAndriodClients(pids, cmdToVideoSvr, channelId);

                PushQueue("播放视频");

                GlobalUtils.VideoSvrArg = cmdToVideoSvr.arg;



                TimeSpan beforeSendToAllClient = new TimeSpan(DateTime.Now.Ticks);
                

                string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(GlobalUtils.ReadyToSentClientData);
                hub.Clients(GlobalUtils.ReadyToSentClientIds).sendMessageToClient(jsonDataToClient);

                TimeSpan afterSendToAllClient = new TimeSpan(DateTime.Now.Ticks);

                double offsetTotalMilliseconds = afterSendToAllClient.Subtract(beforeSendToAllClient).Duration().TotalMilliseconds;


                int bufferTime = (int)(4000 - offsetTotalMilliseconds);

                cmdToVideoSvr.arg.buffer = cmdToVideoSvr.arg.buffer = isScheduled ? bufferTime.ToString(): (bufferTime-2000).ToString();

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
                hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonData);


                GlobalUtils.IsChannelPlaying = true;
            }


        }

        public static void SendStopRoRepeatCommand(string commandType, IHubConnectionContext hub)
        {
            string errorrNotOpenVideoSvr = "视频服务器未开启";
            if (GlobalUtils.IsChannelPlaying)
            {
                string videoSvrId = GlobalUtils.GetVideoServerConnectionIds();

                if (string.IsNullOrWhiteSpace(videoSvrId))
                {
                    List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                    hub.Clients(alPCIds).sendResultBrowserClient(errorrNotOpenVideoSvr, "200");

                    hub.Clients(alPCIds).sendResultBrowserClientNoticeStatus(errorrNotOpenVideoSvr, "200");

                }

                else
                {
                    VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                    cmdToVideoSvr.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


                    GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();

                    cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;

                    VideoOperAndriodClientCommand cmdToAndroidClient = CreateAndroidCommandForStopRepeat(commandType);

                    cmdToVideoSvr.arg = (VideoServerOperArg)GlobalUtils.VideoSvrArg;

                    PushQueue(commandType == "1" ? "停止" : "循环播放");

                    string jsonDataToVideoSvr = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
                    hub.Client(videoSvrId).sendMessageToClient(jsonDataToVideoSvr);


                    string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToAndroidClient);

                    hub.Clients(GlobalUtils.ReadyToSentClientIds).sendMessageToClient(jsonDataToClient);


                    if (commandType == "1")
                    {
                        GlobalUtils.IsChannelPlaying = false;
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
        private static void PushQueue(string cmdText)
        {

            GlobalUtils.CommandQueues.Add(new QueueItem() { IpAddressStr = GlobalUtils.VideoServerIPAddress, GuidIdStr = GlobalUtils.CurrentVideoGuidId, CommandStr = cmdText });

            foreach (var ip in GlobalUtils.ReadyToSentClientIPs)
            {
                GlobalUtils.CommandQueues.Add(new QueueItem() { IpAddressStr = ip, GuidIdStr = GlobalUtils.CurrentClientGuidId, CommandStr = cmdText });
            }
        }


        private static void CreatePlayCommandForAndriodClients(List<ProgramInfo> pids, VideoServerOperCommand cmdToVideoSvr, string channelId)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = CommandTypeEnum.PLAYVEDIO;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();
            GlobalUtils.CurrentClientGuidId = dataSendToAndroidClient.guidId;


            dataSendToAndroidClient.arg = new VideoOperAndriodClientArg();


            dataSendToAndroidClient.arg.bitRate = pids[0].MappingFiles[0].BitRate;

            dataSendToAndroidClient.arg.mediaType = GlobalUtils.CheckIfAudio(pids[0].MappingFiles[0].FileName) ? 1 : 2;

            dataSendToAndroidClient.arg.streamSrcs = cmdToVideoSvr.arg.streamSrcs;

            dataSendToAndroidClient.arg.udpBroadcastAddress = cmdToVideoSvr.arg.udpBroadcastAddress;

            GlobalUtils.ReadyToSentClientData = dataSendToAndroidClient;


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
                GlobalUtils.ReadyToSentClientIPs = needSentClientIpAddresses;
                GlobalUtils.ReadyToSentClientIds = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses);
            }
            else
            {
                GlobalUtils.ReadyToSentClientIPs = new List<string>();
                GlobalUtils.ReadyToSentClientIds = new List<string>();
            }
        }



    }
}