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



        public static void SendPlayCommand(string channelId, string channelName, string[] programeIds, IHubConnectionContext hub, string scheduleTaskGuidId, string scheduleTime, bool isRepeat)
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

                        GlobalUtils.AddLogs(hub, "手动操作", channelName + "正在播放中,请先停止");
                        hub.Clients(alPCIds).sendManualPlayStatus("正在播放中,请先停止", "200");
                        return;
                    }

                    foreach (var sTask in GlobalUtils.RunningSchudules)
                    {
                        //优先手工播放
                        if (sTask.ChannelId == channelId)
                        {
                            // SendStopRoRepeatCommand("1", hub, sTask.GuidId, channelId,);

                            SendStopRoRepeatCommand(channelId, channelName, true, hub, sTask.GuidId, sTask.RunningTime);
                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = scheduleTaskGuidId;

                            cr.errorCode = "202";

                            cr.message = "计划停止，优先手工播放(通道名称+" + GlobalUtils.ChannelManuallyPlayingChannelName + ")";


                            GlobalUtils.AddLogs(hub, "计划任务", GlobalUtils.ChannelManuallyPlayingChannelName + "计划已停止，优先手工播放,运行时间：" + sTask.RunningTime);

                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));

                            break;


                        }
                    }

                }


                if (isSchedule && GlobalUtils.IsChannelManuallyPlaying && channelId == GlobalUtils.ChannelManuallyPlayingChannelId)
                {
                    //优先手工播放
                    ComuResponseBase cr = new ComuResponseBase();

                    cr.guidId = scheduleTaskGuidId;

                    cr.errorCode = "202";

                    cr.message = "计划失败，优先手动播放(通道名称+" + GlobalUtils.ChannelManuallyPlayingChannelName + ")";

                    GlobalUtils.AddLogs(hub, "计划任务", channelName + "播放计划失败，优先手工播放，运行时间：" + scheduleTime);

                    hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));

                    return;
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

                            cr.message = errorrNotOpenVideoSvr;
                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));

                            GlobalUtils.AddLogs(hub, "计划任务", channelName + "播放计划失败，" + errorrNotOpenVideoSvr + ", 运行时间：" + scheduleTime);
                        }
                    }
                    else
                    {
                        List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();


                        GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动播放失败, " + errorrNotOpenVideoSvr);

                        hub.Clients(alPCIds).sendManualPlayStatus(errorrNotOpenVideoSvr, "200");
                    }


                    return;
                }

                else
                {
                    if (isSchedule)
                    {

                        string cidToStop = string.Empty;

                        string scheduleTimeToStop = string.Empty;

                        string guidIdToStop = string.Empty;
                        foreach (var sTask in GlobalUtils.RunningSchudules)
                        {
                            //取消上个同个通道的计划
                            if (sTask.ChannelId == channelId)
                            {
                                cidToStop = channelId;
                                scheduleTimeToStop = sTask.RunningTime;
                                guidIdToStop = sTask.GuidId;
                                break;

                            }
                        }

                        if (!string.IsNullOrWhiteSpace(cidToStop))
                        {
                            //SendStopRoRepeatCommand("1", hub, guidIdToStop, channelId);
                            SendStopRoRepeatCommand(channelId, channelName, true, hub, guidIdToStop, scheduleTimeToStop);
                        }
                    }

                    VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                    cmdToVideoSvr.commandType = CommandTypeEnum.PLAYVEDIO;

                    List<ProgramInfo> pids = GlobalUtils.ProgramBLLInstance.GetProgramByIds(programeIds);


                    cmdToVideoSvr.arg = new VideoServerOperArg();


                    cmdToVideoSvr.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");

                    cmdToVideoSvr.arg.isRepeat = isRepeat ? 1 : 0;

                    GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();


                    cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;


                    cmdToVideoSvr.arg.streamName = GlobalUtils.StreamNameBase + channelId;

                    cmdToVideoSvr.arg.streamSrcs = new List<string>();


                    int maxBitRate = 0;

                    int outBitRate = 0;

                    foreach (var pi in pids)
                    {
                        if (pi.MappingFiles != null && pi.MappingFiles.Count > 0)
                        {
                            foreach (var file in pi.MappingFiles)
                            {
                                if (!string.IsNullOrWhiteSpace(file.BitRate) && int.TryParse(file.BitRate, out outBitRate))
                                {
                                    maxBitRate = outBitRate > maxBitRate ? outBitRate : maxBitRate;
                                }

                                cmdToVideoSvr.arg.streamSrcs.Add(file.FileName);
                            }
                        }
                    }

                    int intCID = 0;

                    int port = 0;
                    if (int.TryParse(channelId, out intCID))
                    {
                        port = 1234 + intCID;
                    }

                    cmdToVideoSvr.arg.udpBroadcastAddress = "udp://229.0.0.1:" + port.ToString();


                    List<string> clientsIpToSend = new List<string>();

                    List<string> clientsConectionIdToSend = new List<string>();

                    VideoOperAndriodClientCommand clientsDatraToSend = new VideoOperAndriodClientCommand();

                    CreatePlayCommandForAndriodClients(pids, cmdToVideoSvr, channelId, out clientsIpToSend, out clientsConectionIdToSend, out clientsDatraToSend, maxBitRate);



                    QueueCommandType type = QueueCommandType.NONE;

                    if (isSchedule)
                    {
                        type = QueueCommandType.SCHEDULEPLAY;
                    }
                    else
                    {
                        GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
                        type = QueueCommandType.MANAULLYPLAY;
                    }

                    PushQueue(type, clientsIpToSend, isSchedule, channelName, scheduleTime);


                    GlobalUtils.VideoSvrArg = cmdToVideoSvr.arg;


                    ParamConfig pc = GlobalUtils.ParamConfigBLLInstance.GetParamConfig();

                    int intBufTime = isSchedule ? pc.BufferTimeForSchedule : pc.BufferTimeForManualPlay;


                    TimeSpan beforeSendToAllClient = new TimeSpan(DateTime.Now.Ticks);

                    System.Diagnostics.Debug.WriteLine("Play Command Send Bfore " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId);



                    string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(clientsDatraToSend);
                    hub.Clients(clientsConectionIdToSend).sendMessageToClient(jsonDataToClient);


                    System.Diagnostics.Debug.WriteLine("Paly json To Android client: " + jsonDataToClient + DateTime.Now.ToString("HH:mm:ss fff"));
                    TimeSpan afterSendToAllClient = new TimeSpan(DateTime.Now.Ticks);

                    double offsetTotalMilliseconds = afterSendToAllClient.Subtract(beforeSendToAllClient).Duration().TotalMilliseconds;


                    int bufferTime = (int)(intBufTime - offsetTotalMilliseconds);

                    cmdToVideoSvr.arg.buffer = cmdToVideoSvr.arg.buffer = !string.IsNullOrWhiteSpace(scheduleTaskGuidId) ? bufferTime.ToString() : (bufferTime - 2000).ToString();

                    string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);


                    if (!string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
                    {
                        hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonData);
                    }
                    else
                    {
                        GlobalUtils.AddLogs(hub, "系统异常", "视频服务器突然关闭");
                        return;

                    }

                    System.Diagnostics.Debug.WriteLine("Paly json To video Server: " + jsonData + DateTime.Now.ToString("HH:mm:ss fff"));
                    System.Diagnostics.Debug.WriteLine("Play Command Send AFTER " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId);




                    new Thread(ProcessTimeOutRequest).Start(hub);

                    if (!isSchedule)
                    {
                        GlobalUtils.ChannelManuallyPlayingChannelId = channelId;

                        GlobalUtils.ChannelManuallyPlayingPids = programeIds;

                        GlobalUtils.ChannelManuallyPlayingChannelName = channelName;

                        GlobalUtils.IsChannelManuallyPlaying = true;


                        GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动播放成功");

                    }
                    else
                    {
                        GlobalUtils.RunningSchudules.Add(new ScheduleRunningItem { ChannelId = channelId, GuidId = scheduleTaskGuidId, RunningTime = scheduleTime });
                        System.Diagnostics.Debug.WriteLine("Add Schedule Task " + channelId + " Now Count Is:" + GlobalUtils.RunningSchudules.Count);

                        ComuResponseBase cr = new ComuResponseBase();

                        cr.guidId = scheduleTaskGuidId;

                        cr.errorCode = "181";

                        cr.message = "StartVideo";

                        if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                        {

                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                            GlobalUtils.AddLogs(hub, "计划任务", channelName + "运行播放计划成功，运行时间：" + scheduleTime);

                        }



                    }
                }

            }






        }

        private static void ProcessTimeOutRequest(object hub)
        {
            Thread.Sleep(4000);

            lock (GlobalUtils.objectLockSchduleQueueItem)
            {
                IHubConnectionContext hubContent = hub as IHubCallerConnectionContext;
                List<QueueItem> queueToRemoved = new List<QueueItem>();
                foreach (var que in GlobalUtils.CommandQueues)
                {
                    TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);

                    TimeSpan tsSubmited = new TimeSpan(que.PushTicks);
                    if (ts.Subtract(tsSubmited).Duration().TotalMilliseconds >= 4000)
                    {
                        queueToRemoved.Add(que);

                    }
                }

                if (queueToRemoved != null && queueToRemoved.Count > 0)
                {
                    foreach (QueueItem item in queueToRemoved)
                    {

                        string ipToDisplay = string.Empty;
                        if (item.IsVideoServer)
                        {

                            ipToDisplay = " 视频服务器";
                        }
                        else
                        {
                            ipToDisplay = " 终端:" + item.IpAddressStr;

                        }

                        string strCmdType = GlobalUtils.GetCommandTextGetByType(item.CommandType);
                        if (item.IsScheduled)
                        {

                            GlobalUtils.AddLogs(hubContent, "计划任务", item.ChannelName + strCmdType + ipToDisplay + "操作超时, 计划时间:" + item.ScheduledTime);
                        }
                        else
                        {
                            GlobalUtils.AddLogs(hubContent, "手动操作", item.ChannelName + strCmdType + ipToDisplay + "操作超时");
                        }

                        GlobalUtils.CommandQueues.Remove(item);
                        //  System.Diagnostics.Debug.WriteLine("Remove Command No Response: Now count is :" + GlobalUtils.CommandQueues.Count);
                    }

                }
            }
        }

        public static void SendStopRoRepeatCommand(string channelId, string channelName, bool isWantToStop, IHubConnectionContext hub, string scheduleTaskGuidId, string scheduleTime, bool isSendToVideoSvr = true)
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

                    cr.message = errorrNotOpenVideoSvr;

                    if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                    {
                        GlobalUtils.AddLogs(hub, "计划任务", channelName + "结束播放计划失败，" + errorrNotOpenVideoSvr + ",运行时间：" + scheduleTime);
                        hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                    }

                }
                else
                {
                    List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                    hub.Clients(alPCIds).sendManualPlayStatus(errorrNotOpenVideoSvr, "200");

                    GlobalUtils.AddLogs(hub, "手动操作", channelName + "停止播放操作失败，" + errorrNotOpenVideoSvr + scheduleTime);
                }

            }

            else
            {
                SendOutStopRepeatCommandToServerAndClient(channelId, channelName, isWantToStop, hub, isSchedule, scheduleTime,isSendToVideoSvr);
            }




        }

        private static void SendOutStopRepeatCommandToServerAndClient(string channelId, string channelName, bool isWantToStop, IHubConnectionContext hub,
             bool isSchedule, string scheduleTime, bool isSendToVideoSvr = true)
        {

            lock (GlobalUtils.PublicObjectForLock)
            {
                VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                cmdToVideoSvr.commandType = isWantToStop ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


                GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();

                cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;


                cmdToVideoSvr.arg = new VideoServerOperArg();

                cmdToVideoSvr.arg.streamName = GlobalUtils.StreamNameBase + channelId;


                if (!isSchedule)
                {
                    GlobalUtils.ChannelManuallyPlayingIsRepeat = !GlobalUtils.ChannelManuallyPlayingIsRepeat;
                    cmdToVideoSvr.arg.isRepeat = GlobalUtils.ChannelManuallyPlayingIsRepeat ? 1 : 0;
                }

                List<string> clientsIpToSend = new List<string>();

                List<string> clientsConectionIdToSend = new List<string>();

                VideoOperAndriodClientCommand clientsDataToSend = new VideoOperAndriodClientCommand();

                if (isWantToStop)
                {

                    CreateCommandForStopRepeatToClients(cmdToVideoSvr.commandType, channelId, out clientsIpToSend, out clientsConectionIdToSend, out clientsDataToSend);


                    clientsDataToSend.arg = new VideoOperAndriodClientArg();

                }

                QueueCommandType cmdType = QueueCommandType.NONE;

                if (isSchedule)
                {
                    cmdType = QueueCommandType.SCHEDULESTOP;

                }
                else
                {

                    cmdType = isWantToStop ? QueueCommandType.MANAULLYSTOP : QueueCommandType.MANAULLYREPEAT;
                }



                PushQueue(cmdType, clientsIpToSend, isSchedule, channelName, scheduleTime,isSendToVideoSvr);


                System.Diagnostics.Debug.WriteLine("Stop Command Send BEFORE " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId);

                string jsonDataToVideoSvr = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);

                if (!string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
                {
                    if (isSendToVideoSvr)
                    {
                        hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonDataToVideoSvr);

                        System.Diagnostics.Debug.WriteLine("STOP json To Video Sever: " + jsonDataToVideoSvr + DateTime.Now.ToString("HH:mm:ss fff"));
                    }
                }
                else
                {
                    GlobalUtils.AddLogs(hub, "系统异常", "视频服务器突然关闭");
                    return;

                }



                if (isWantToStop)
                {

                    string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(clientsDataToSend);

                    hub.Clients(clientsConectionIdToSend).sendMessageToClient(jsonDataToClient);

                    System.Diagnostics.Debug.WriteLine("STOP json To Adnriod Client: " + jsonDataToClient + DateTime.Now.ToString("HH:mm:ss fff"));
                }

                System.Diagnostics.Debug.WriteLine("Stop Command Send AFTER " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId);

                new Thread(ProcessTimeOutRequest).Start(hub);

                if (!isSchedule)
                {

                    GlobalUtils.ChannelManuallyPlayingChannelId = string.Empty;
                    GlobalUtils.ChannelManuallyPlayingPids = null;
                    GlobalUtils.ChannelManuallyPlayingChannelName = string.Empty;
                    GlobalUtils.IsChannelManuallyPlaying = false;

                    GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动停止成功");
                }
                else
                {

                    ///Remove schudle task
                    ScheduleRunningItem itemToRemove = null;

                    foreach (var sTask in GlobalUtils.RunningSchudules)
                    {
                        if (sTask.ChannelId == channelId)
                        {
                            itemToRemove = sTask;
                        }
                    }


                    if (GlobalUtils.IsChannelManuallyPlaying)
                    {
                        if (channelId == GlobalUtils.ChannelManuallyPlayingChannelId)
                        {
                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = itemToRemove.GuidId;

                            cr.errorCode = "200";

                            cr.message = "计划停止失败，手工播放中";

                            if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                            {

                                GlobalUtils.AddLogs(hub, "计划任务", channelName + "结束播放计划执行失败，手工播放中,运行时间：" + scheduleTime);
                                hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                            }

                            return;
                        }
                    }


                    if (itemToRemove != null)
                    {

                        ComuResponseBase cr = new ComuResponseBase();

                        cr.guidId = itemToRemove.GuidId;

                        cr.errorCode = "180";

                        cr.message = "StopVideo";

                        if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                        {

                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                        }


                        GlobalUtils.RunningSchudules.Remove(itemToRemove);

                        GlobalUtils.AddLogs(hub, "计划任务", channelName + "结束播放计划成功,运行时间：" + scheduleTime);

                        System.Diagnostics.Debug.WriteLine("Removing Schedule Task " + itemToRemove.ChannelId + " Now Count Is:" + GlobalUtils.RunningSchudules.Count);
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


        private static void PushQueue(QueueCommandType cmdType, List<string> clientIps, bool isScheduled, string channelName, string scheduleTime, bool isSendToVideoSvr = true)
        {
            long currentTicks = DateTime.Now.Ticks;

            if (isSendToVideoSvr)
            {

                GlobalUtils.CommandQueues.Add(new QueueItem() { IsVideoServer = true, ScheduledTime = scheduleTime, ChannelName = channelName, IsScheduled = isScheduled, PushTicks = currentTicks, IpAddressStr = GlobalUtils.VideoServerIPAddress, GuidIdStr = GlobalUtils.CurrentVideoGuidId, CommandType = cmdType });
            }

            //NO need send to client when is repeat operation.
            if (cmdType != QueueCommandType.MANAULLYREPEAT)
            {
                foreach (var ip in clientIps)
                {
                    GlobalUtils.CommandQueues.Add(new QueueItem() { ScheduledTime = scheduleTime, ChannelName = channelName, IsScheduled = isScheduled, PushTicks = currentTicks, IpAddressStr = ip, GuidIdStr = GlobalUtils.CurrentClientGuidId, CommandType = cmdType });
                }
            }
        }


        private static void CreatePlayCommandForAndriodClients(List<ProgramInfo> pids, VideoServerOperCommand cmdToVideoSvr, string channelId,
            out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out VideoOperAndriodClientCommand dataToSend, int maxBitRate)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = CommandTypeEnum.PLAYVEDIO;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();
            GlobalUtils.CurrentClientGuidId = dataSendToAndroidClient.guidId;


            dataSendToAndroidClient.arg = new VideoOperAndriodClientArg();


            if (pids != null && pids.Count > 0)
            {


                dataSendToAndroidClient.arg.bitRate = maxBitRate.ToString();

                dataSendToAndroidClient.arg.mediaType = GlobalUtils.CheckIfAudio(pids[0].MappingFiles[0].FileName) ? 1 : 2;

                dataSendToAndroidClient.arg.streamSrcs = cmdToVideoSvr.arg.streamSrcs;

                dataSendToAndroidClient.arg.udpBroadcastAddress = cmdToVideoSvr.arg.udpBroadcastAddress;
            }

            dataToSend = dataSendToAndroidClient;

            GetClientIpAndIdList(channelId, out ipsNeedToSend, out idsNeedToSend);
        }


        private static void CreateCommandForStopRepeatToClients(CommandTypeEnum cmdType, string channelId,
           out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out VideoOperAndriodClientCommand dataToSend)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = cmdType;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();
            GlobalUtils.CurrentClientGuidId = dataSendToAndroidClient.guidId;


            dataToSend = dataSendToAndroidClient;



            GetClientIpAndIdList(channelId, out ipsNeedToSend, out idsNeedToSend);
        }

        private static void GetClientIpAndIdList(string channelId, out List<string> ipsNeedToSend, out List<string> idsNeedToSend)
        {
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
            }       }

     


    }
}