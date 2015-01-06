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
using System.IO;
namespace MediaMgrSystem
{

    public static class SendLogic
    {

        public static void SendChangeChannelSchedule(IHubConnectionContext hub, string cid, string channelName)
        {
            string channelId = string.Empty;
            string guidIdToStop = string.Empty;
            string scheduleTimeToStop = string.Empty;
            foreach (var sTask in GlobalUtils.RunningSchudules)
            {
                //取消上个同个通道的计划
                if (sTask.ChannelId == cid)
                {
                    channelId = cid;
                    scheduleTimeToStop = sTask.RunningTime;
                    guidIdToStop = sTask.GuidId;
                    break;

                }
            }

            if (!string.IsNullOrWhiteSpace(channelId))
            {
                //SendStopRoRepeatCommand("1", hub, guidIdToStop, channelId);
                SendStopRoRepeatCommand(channelId, channelName, true, hub, guidIdToStop, scheduleTimeToStop, BusinessType.AUDITBROADCAST);
            }
        }
        public static void SendChangeIpAddressAndServerUrl(IHubConnectionContext hub, string oldIpAddress, string newIpAddress, string serverUrl)
        {

            List<string> ipsNeedToSend = null;
            List<string> idsNeedToSend = null;
            List<string> ipReallySent = null;
            ipsNeedToSend = new List<string>();

            ipsNeedToSend.Add(oldIpAddress);

            idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(ipsNeedToSend, SingalRClientConnectionType.ANDROID, out ipReallySent);


            DeviceOperationCommand cmd = new DeviceOperationCommand();


            cmd.commandType = CommandTypeEnum.DEVICE_OPER_CHANGE_IP_ADDRESS;

            cmd.guidId = Guid.NewGuid().ToString();


            QueueCommandType queueCommandType = QueueCommandType.DEVICE_OPER_CHANGE_IP_ADDRESS;

            PushQueue(queueCommandType, ipsNeedToSend, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, cmd.guidId, false, newIpAddress);



            cmd.arg = new DeviceOperationCommandArg();


            cmd.arg.newIpAddress = newIpAddress;
            cmd.arg.serverUrl = serverUrl;

            hub.Clients(idsNeedToSend).sendMessageToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));

            new Thread(ProcessTimeOutRequest).Start(hub);



        }
        public static void SendDeviceOperCommand(IHubConnectionContext hub, string cmdStr, string groupId, string deviceIpAddress, string scheduleTurnOnTime, string scheduleShutDownTime, int isEnabled, string volValue)
        {

            List<string> ipsNeedToSend = null;
            List<string> idsNeedToSend = null;
            if (!string.IsNullOrEmpty(groupId))
            {
                List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetGroupById(groupId);


                List<string> needSentClientIpAddresses = new List<string>();
                if (gis != null && gis.Count > 0)
                {
                    foreach (var gi in gis)
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
                    List<string> ipReallySent = new List<string>();
                    idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.ANDROID, out ipReallySent);
                }
                else
                {
                    ipsNeedToSend = new List<string>();
                    idsNeedToSend = new List<string>();
                }

            }
            else
            {
                ipsNeedToSend = new List<string>();

                ipsNeedToSend.Add(deviceIpAddress);
                List<string> ipReallySent = new List<string>();
                idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(ipsNeedToSend, SingalRClientConnectionType.ANDROID, out ipReallySent);

            }


            DeviceOperationCommand cmd = new DeviceOperationCommand();


            CommandTypeEnum cmdType = (CommandTypeEnum)Enum.Parse(typeof(CommandTypeEnum), cmdStr);

            cmd.commandType = cmdType;

            cmd.guidId = Guid.NewGuid().ToString();


            QueueCommandType queueCommandType = (QueueCommandType)Enum.Parse(typeof(QueueCommandType), cmdStr);

            PushQueue(queueCommandType, ipsNeedToSend, false, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, cmd.guidId, false,"",volValue);


            cmd.arg = new DeviceOperationCommandArg();


            cmd.arg.scheduleTurnOnTime = scheduleTurnOnTime;

            cmd.arg.volValue = volValue;
            cmd.arg.scheduleShutDownTime = scheduleShutDownTime;

            cmd.arg.isEnabled = isEnabled;



            try
            {
                hub.Clients(idsNeedToSend).sendMessageToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));
            }
            catch (Exception ex)
            {
                GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);
            }
            new Thread(ProcessTimeOutRequest).Start(hub);


        }

        public static void SendPlayCommand(string channelId, string channelName, string[] programeIds, IHubConnectionContext hub, string scheduleTaskGuidId, string scheduleTime, bool isRepeat, BusinessType bType, string strScheduleTaskPriority = "")
        {
            lock (GlobalUtils.PublicObjectForLockPlay)
            {

                try
                {



                    bool isSchedule = !string.IsNullOrWhiteSpace(scheduleTaskGuidId);

                    string errorrNotOpenVideoSvr = "视频服务器未开启";

                    if (!isSchedule)
                    {

                        foreach (var sTask in GlobalUtils.RunningSchudules)
                        {
                            if (sTask.ChannelId == channelId)
                            { //优先手工播放/如何不是打铃的话
                                //if (sTask.Priority != "1")
                                //{
                                // SendStopRoRepeatCommand("1", hub, sTask.GuidId, channelId,);


                                SendStopRoRepeatCommand(channelId, channelName, true, hub, sTask.GuidId, sTask.RunningTime, bType);
                                ComuResponseBase cr = new ComuResponseBase();

                                cr.guidId = scheduleTaskGuidId + "," + channelId;

                                cr.errorCode = "202";

                                cr.message = "计划停止，优先手工播放(通道名称+" + channelName + ")";


                                GlobalUtils.AddLogs(hub, "计划任务", channelName + "计划已停止，优先手工播放,运行时间：" + sTask.RunningTime);

                                hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                                //等待2秒再发给终端
                                Thread.Sleep(2000);
                                break;



                            }
                        }

                    }


                    if (isSchedule)
                    {

                        ManualPlayItem mpForChannel = GlobalUtils.GetManaulPlayItemByChannelId(channelId);

                        if (mpForChannel != null && mpForChannel.IsPlaying)
                        {
                            //if (strScheduleTaskPriority != "1")
                            //{
                            //优先手工播放
                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = scheduleTaskGuidId + "," + channelId;

                            cr.errorCode = "202";

                            cr.message = "计划失败，优先手动播放(通道名称+" + mpForChannel.ChannelName + ")";

                            GlobalUtils.AddLogs(hub, "计划任务", channelName + "播放计划失败，优先手工播放，运行时间：" + scheduleTime);

                            hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));

                            return;
                        }

                    }


                    if (string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
                    {

                        if (isSchedule)
                        {

                            if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                            {
                                ComuResponseBase cr = new ComuResponseBase();

                                cr.guidId = scheduleTaskGuidId + "," + channelId;

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

                            ManualPlayItem item = GlobalUtils.GetManaulPlayItemByChannelId(channelId);
                            if (item == null)
                            {
                                item = new ManualPlayItem();
                                item.ChannelId = channelId;
                                item.ChannelName = channelName;
                                item.PlayingPids = programeIds;
                                item.PlayingFunction = bType;
                            }

                            GlobalUtils.SendManuallyClientNotice(hub, errorrNotOpenVideoSvr, "200", item);
                            // hub.Clients(alPCIds).sendManualPlayStatus(errorrNotOpenVideoSvr, "200", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());
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
                                SendStopRoRepeatCommand(channelId, channelName, true, hub, guidIdToStop, scheduleTimeToStop, bType);
                            }
                        }

                        VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                        cmdToVideoSvr.commandType = CommandTypeEnum.PLAYVEDIO;

                        List<ProgramInfo> pids = GlobalUtils.ProgramBLLInstance.GetProgramByIds(programeIds);


                        cmdToVideoSvr.arg = new VideoServerOperArg();



                        cmdToVideoSvr.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");

                        cmdToVideoSvr.arg.isRepeat = isRepeat ? 1 : 0;




                        cmdToVideoSvr.guidId = Guid.NewGuid().ToString();


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



                        if (bType == BusinessType.AUDITBROADCAST)
                        {
                            cmdToVideoSvr.arg.udpBroadcastAddress = "udp://239.0.0.1:" + port.ToString();
                        }
                        else
                        {
                            cmdToVideoSvr.arg.udpBroadcastAddress = "udp://239.0.0.2:" + port.ToString();
                        }


                        List<string> clientsIpToSend = new List<string>();

                        List<string> clientsConectionIdToSend = new List<string>();

                        List<string> ipReallySent = new List<string>();

                        VideoOperAndriodClientCommand clientsDatraToSend = new VideoOperAndriodClientCommand();

                        List<GroupInfo> channelGroup = GlobalUtils.GroupBLLInstance.GetGroupByChannelId(channelId, bType);

                        bool isNotBindAnyGroup = channelGroup.Count <= 0;

                        QueueCommandType type = QueueCommandType.NONE;

                        if (isSchedule)
                        {
                            type = QueueCommandType.SCHEDULEPLAY;
                        }
                        else
                        {
                            type = QueueCommandType.MANAULLYPLAY;

                        }

                        List<GroupInfo> giToRemove = new List<GroupInfo>();

                        List<MediaMgrSystem.CallerEncoderControlLogic.NeedStopVideoEncoderTask> needStopVideoEncoders = new List<MediaMgrSystem.CallerEncoderControlLogic.NeedStopVideoEncoderTask>();

                        if (GlobalUtils.GlobalGroupBusinessStatus.Count > 0 && !isNotBindAnyGroup)
                        {
                            foreach (var gi in channelGroup)
                            {

                                foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                                {
                                    if (grr.GroupId == gi.GroupId)
                                    {

                                        if (grr.TypeRunning == BusinessTypeForGroup.AudioEncoder)
                                        {
                                            //无能为力，呼叫台最大
                                            giToRemove.Add(gi);

                                            GlobalUtils.AddLogs(hub, isSchedule ? "计划任务" : "手工操作", "播放时" + gi.GroupName + GlobalUtils.GetRunningBusinessTypeDesp(grr.TypeRunning));
                                        }

                                        else if (grr.TypeRunning == BusinessTypeForGroup.VideoEncoder)
                                        {
                                            //停止视频编码

                                            bool isEncoderExsting = false;
                                            for (int i = 0; i < needStopVideoEncoders.Count; i++)
                                            {
                                                if (needStopVideoEncoders[i].encoderId == grr.encoderId)
                                                {
                                                    isEncoderExsting = true;
                                                    if (needStopVideoEncoders[i].groups != null)
                                                    {
                                                        if (!needStopVideoEncoders[i].groups.Contains(gi))
                                                        {
                                                            needStopVideoEncoders[i].groups.Add(gi);
                                                        }
                                                    }
                                                }
                                            }

                                            if (!isEncoderExsting)
                                            {

                                                MediaMgrSystem.CallerEncoderControlLogic.NeedStopVideoEncoderTask nst = new MediaMgrSystem.CallerEncoderControlLogic.NeedStopVideoEncoderTask();
                                                nst.encoderId = grr.encoderId;

                                                nst.groups = new List<GroupInfo>();

                                                nst.groups.Add(gi);
                                                needStopVideoEncoders.Add(nst);

                                            }

                                            //  needStopVideoEncoders.Add(gi);


                                            //GlobalUtils.AddLogs(hub, "计划任务", "播放时" + gi.GroupName + GlobalUtils.CheckRunningBusinessTypeDesp(grr.typeRunning));
                                        }
                                    }
                                }


                            }




                            if (needStopVideoEncoders.Count > 0)
                            {
                                foreach (var task in needStopVideoEncoders)
                                {
                                    VideoEncoderControlLogic.SendVideoEncoderOperation(hub, task.encoderId, false, false, task.groups);
                                }
                                // VideoEncoderControlLogic.SendVideoEncoderOperation(hub, "", false, true, needStopVideoEncoders);
                            }

                            if (giToRemove.Count > 0)
                            {
                                foreach (var gRemote in giToRemove)
                                {
                                    channelGroup.Remove(gRemote);

                                }
                            }
                        }


                        if (channelGroup.Count <= 0)
                        {

                            if (isSchedule)
                            {

                                if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                                {
                                    ComuResponseBase cr = new ComuResponseBase();

                                    cr.guidId = scheduleTaskGuidId + "," + channelId;

                                    cr.errorCode = "2100";
                                    if (isNotBindAnyGroup)
                                    {
                                        cr.message = "通道没有关联任何分组";
                                    }
                                    else
                                    {
                                        cr.message = "所有组被占用";
                                    }
                                    hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));

                                    GlobalUtils.AddLogs(hub, "计划任务", channelName + "播放计划失败，" + cr.message + ", 运行时间：" + scheduleTime);
                                }
                            }
                            else
                            {
                                List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();

                                string messsageStr = string.Empty;
                                if (isNotBindAnyGroup)
                                {
                                    messsageStr = "通道没有关联任何分组";
                                }
                                else
                                {
                                    messsageStr = "所有组被占用";
                                }

                                GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动播放失败, " + messsageStr);

                                ManualPlayItem item = GlobalUtils.GetManaulPlayItemByChannelId(channelId);
                                if (item == null)
                                {
                                    item = new ManualPlayItem();
                                    item.ChannelId = channelId;
                                    item.ChannelName = channelName;
                                    item.PlayingPids = programeIds;
                                    item.PlayingFunction = bType;
                                }

                                GlobalUtils.SendManuallyClientNotice(hub, messsageStr, "200", item);
                                // hub.Clients(alPCIds).sendManualPlayStatus(errorrNotOpenVideoSvr, "200", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());
                            }

                            return;
                        }

                        List<GroupInfo> gOut = new List<GroupInfo>();

                        CreatePlayCommandForAndriodClients(pids, cmdToVideoSvr, channelId, out clientsIpToSend, out clientsConectionIdToSend, out ipReallySent, out clientsDatraToSend, maxBitRate, bType, out gOut, channelGroup);



                        PushQueue(type, clientsIpToSend, isSchedule, channelId, channelName, scheduleTaskGuidId, scheduleTime, cmdToVideoSvr.guidId, clientsDatraToSend.guidId);


                        // cmdToVideoSvr.arg;


                        ParamConfig pc = GlobalUtils.ParamConfigBLLInstance.GetParamConfig();

                        int intBufTime = isSchedule ? pc.BufferTimeForSchedule : pc.BufferTimeForManualPlay;


                        TimeSpan beforeSendToAllClient = new TimeSpan(DateTime.Now.Ticks);

                        string logs = "Play Command Send Bfore " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId;
                        System.Diagnostics.Debug.WriteLine(logs);

                        GlobalUtils.WriteDebugLogs(logs);


                        string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(clientsDatraToSend);
                        hub.Clients(clientsConectionIdToSend).sendMessageToClient(jsonDataToClient);

                        foreach (var css in ipReallySent)
                        {

                            bool isExsting = false;

                            foreach (var pd in GlobalUtils.PlayingDevices)
                            {
                                if (pd.IpAddress == css)
                                {
                                    isExsting = true;
                                    break;
                                }
                            }


                            if (!isExsting)
                            {
                                bool isAudio = clientsDatraToSend.arg.mediaType == 1;

                                if (isSchedule)
                                {
                                    isAudio = true;
                                }

                                GlobalUtils.PlayingDevices.Add(new PlayDevice { IpAddress = css, IsAudio = isAudio });
                            }
                        }



                        logs = "Paly json To Android client: " + jsonDataToClient + DateTime.Now.ToString("HH:mm:ss fff");

                        System.Diagnostics.Debug.WriteLine(logs);
                        GlobalUtils.WriteDebugLogs(logs);
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

                        string str = "Paly json To video Server: " + jsonData + DateTime.Now.ToString("HH:mm:ss fff");
                        System.Diagnostics.Debug.WriteLine(str);
                        GlobalUtils.WriteDebugLogs(str);
                        str = "Play Command Send AFTER " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId;
                        System.Diagnostics.Debug.WriteLine(str);
                        GlobalUtils.WriteDebugLogs(str);

                        new Thread(ProcessTimeOutRequest).Start(hub);

                        if (!isSchedule)
                        {



                            ManualPlayItem mp = new ManualPlayItem();

                            mp.ChannelId = channelId;
                            mp.ChannelName = channelName;

                            mp.PlayingPids = programeIds;

                            mp.PlayingFunction = bType;

                            mp.IsRepeating = false;

                            mp.ChannelGroup = channelGroup;

                            mp.IsPlaying = true;
                            GlobalUtils.AddManualPlayItem(mp);

                 
                            //  GlobalUtils.ChannelManuallyPlayingFunction = bType;

                            //   hub.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus("Play", "0", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());

                            GlobalUtils.SendManuallyClientNotice(hub, "Play", "0", mp);
                            GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动播放成功");

                        }
                        else
                        {
                            GlobalUtils.RunningSchudules.Add(new ScheduleRunningItem { ChannelName = channelName, Priority = strScheduleTaskPriority, ChannelId = channelId, GuidId = scheduleTaskGuidId, RunningTime = scheduleTime, ChannelGroup = channelGroup });
                            System.Diagnostics.Debug.WriteLine("Add Schedule Task " + channelId + " Now Count Is:" + GlobalUtils.RunningSchudules.Count);

                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = scheduleTaskGuidId + "," + channelId;

                            cr.errorCode = "181";

                            cr.message = "StartVideo";

                            if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                            {

                                hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                                GlobalUtils.AddLogs(hub, "计划任务", channelName + "运行播放计划成功，运行时间：" + scheduleTime);

                            }



                        }

                        if (channelGroup != null && channelGroup.Count > 0)
                        {
                            foreach (var c in channelGroup)
                            {
                                GlobalUtils.AddLogs(hub, isSchedule ? "计划任务" : "手动操作", c.GroupName + "组播放成功");

                                GroupBusinessRunning gbr = new GroupBusinessRunning();

                                gbr.GroupId = c.GroupId;
                                gbr.TypeRunning = BusinessTypeForGroup.ManualScheduleTask;

                                gbr.bType = bType;
                                gbr.channelId = channelId;
                                gbr.channelName = channelName;
                                gbr.isSchedule = isSchedule;
                                gbr.scheduleTime = scheduleTime;

                                GlobalUtils.GlobalGroupBusinessStatus.Add(gbr);

                                // { GroupId = c.GroupId, TypeRunning = BusinessTypeForGroup.ManualTask });
                                // GlobalUtils.GlobalGroupBusinessStatus.Add(new
                                // isSendToVideoSvr

                            }
                        }

                        List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

                        hub.Clients(ids).sendRefreshAudioDeviceMessge();
                    }

                }


                catch (Exception ex)
                {
                    try
                    {

                        StreamWriter sw = new StreamWriter(@"c:\bslog.txt", true, Encoding.Default);

                        sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + ex.StackTrace.ToString()));


                        sw.Close();

                        GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);

                    }
                    catch { }


                    // HttpContext.Current.Response.Write(ex.StackTrace);
                }

            }
        }

        private static void ProcessTimeOutRequest(object hub)
        {

            Thread.Sleep(4000);

            lock (GlobalUtils.ObjectLockQueueItem)
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

                        if (item.CommandType == QueueCommandType.DEVICE_OPER_CHANGE_IP_ADDRESS)
                        {
                            DeviceInfo di = GlobalUtils.DeviceBLLInstance.GetADevicesByIPAddress(item.NewAddressStr)[0];

                            di.DeviceIpAddress = item.IpAddressStr;
                            GlobalUtils.DeviceBLLInstance.UpdateDevice(di);

                        }

                        GlobalUtils.CommandQueues.Remove(item);
                        //  System.Diagnostics.Debug.WriteLine("Remove Command No Response: Now count is :" + GlobalUtils.CommandQueues.Count);
                    }

                }
            }

        }

        public static void SendStopRoRepeatCommand(string channelId, string channelName, bool isWantToStop, IHubConnectionContext hub, string scheduleTaskGuidId, string scheduleTime, BusinessType bType, bool isSendToVideoSvr = true, string strScheduleTaskPriority = "")
        {

            try
            {
                bool isSchedule = !string.IsNullOrWhiteSpace(scheduleTaskGuidId);

                string errorrNotOpenVideoSvr = "视频服务器未开启";

                string videoSvrId = GlobalUtils.VideoServerConnectionId;


                string aa = "STOP WS ID= " + GlobalUtils.WindowsServiceConnectionId + " " + "VS ID" + GlobalUtils.VideoServerConnectionId + DateTime.Now.ToString("HH:mm:ss fff");
                System.Diagnostics.Debug.WriteLine(aa);
                GlobalUtils.WriteDebugLogs(aa);

                if (string.IsNullOrWhiteSpace(videoSvrId))
                {
                    aa = "Stop no viedo server " + DateTime.Now.ToString("HH:mm:ss fff");
                    System.Diagnostics.Debug.WriteLine(aa);
                    GlobalUtils.WriteDebugLogs(aa);

                    if (isSchedule)
                    {
                        ComuResponseBase cr = new ComuResponseBase();

                        cr.guidId = scheduleTaskGuidId + "," + channelId;

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
                        // hub.Clients(alPCIds).sendManualPlayStatus(errorrNotOpenVideoSvr, "200", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());

                        ManualPlayItem item = GlobalUtils.GetManaulPlayItemByChannelId(channelId);

                        if (item == null)
                        {
                            item = new ManualPlayItem();
                            item.ChannelId = channelId;
                            item.ChannelName = channelName;
                            item.PlayingPids = null;
                            item.PlayingFunction = bType;
                        }
                        GlobalUtils.SendManuallyClientNotice(hub, errorrNotOpenVideoSvr, "200", item);
                        GlobalUtils.AddLogs(hub, "手动操作", channelName + "停止播放操作失败，" + errorrNotOpenVideoSvr + scheduleTime);
                    }
                }

                else
                {
                    SendOutStopRepeatCommandToServerAndClient(channelId, channelName, isWantToStop, hub, isSchedule, scheduleTime, bType, isSendToVideoSvr);
                }

            }

            catch (Exception ex)
            {
                try
                {

                    StreamWriter sw = new StreamWriter(@"c:\bslog.txt", true, Encoding.Default);

                    sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + ex.StackTrace.ToString()));


                    sw.Close();

                    GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);
                    // GlobalUtils.AddLogs(null, "Exception", @"See c:\bslog.txt");
                    //  GlobalUtils.SendManuallyClientNotice(hub,ex.StackTrace, "200", null);

                }
                catch { }


                // HttpContext.Current.Response.Write(ex.StackTrace);
            }

        }

        public static void SendOutStopRepeatCommandToServerAndClient(string channelId, string channelName, bool isWantToStop, IHubConnectionContext hub,
             bool isSchedule, string scheduleTime, BusinessType bType, bool isSendToVideoSvr = true, List<GroupInfo> stopGroups = null)
        {

            bool isStopClients = false;
            string aa = "Stop before PublicObjectForLockStop viedo server " + DateTime.Now.ToString("HH:mm:ss fff");
            System.Diagnostics.Debug.WriteLine(aa);
            GlobalUtils.WriteDebugLogs(aa);
            lock (GlobalUtils.PublicObjectForLockStop)
            {

                aa = "Stop after PublicObjectForLockStop viedo server " + DateTime.Now.ToString("HH:mm:ss fff");
                System.Diagnostics.Debug.WriteLine(aa);
                GlobalUtils.WriteDebugLogs(aa);

                VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                cmdToVideoSvr.commandType = isWantToStop ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;




                cmdToVideoSvr.guidId = Guid.NewGuid().ToString();


                cmdToVideoSvr.arg = new VideoServerOperArg();

                cmdToVideoSvr.arg.streamName = GlobalUtils.StreamNameBase + channelId;


                if (!isSchedule)
                {
                    bool isRepeat = GlobalUtils.SetManaulPlayItemRepeatOffset(channelId);

                    cmdToVideoSvr.arg.isRepeat = isRepeat ? 1 : 0;
                }

                List<string> clientsIpToSend = new List<string>();

                List<string> clientsConectionIdToSend = new List<string>();

                List<string> ipRealySent = new List<string>();

                VideoOperAndriodClientCommand clientsDataToSend = new VideoOperAndriodClientCommand();
                List<GroupInfo> channelGroups = new List<GroupInfo>();
                if (isWantToStop)
                {
                    channelGroups = GlobalUtils.GroupBLLInstance.GetGroupByChannelId(channelId, bType);

                    clientsDataToSend.arg = new VideoOperAndriodClientArg();

                }


                //only stp spec group
                if (stopGroups != null && stopGroups.Count > 0)
                {
                    isSendToVideoSvr = false;

                    int i = 0;
                    if (GlobalUtils.GlobalGroupBusinessStatus.Count > 0)
                    {

                        foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                        {
                            if (grr.TypeRunning == BusinessTypeForGroup.ManualScheduleTask && grr.channelId == channelId)
                            {
                                i++;
                            }
                        }

                    }

                    if (i == stopGroups.Count)
                    {
                        isSendToVideoSvr = true;
                    }
                    else
                    {
                        isStopClients = true;
                    }


                    List<GroupInfo> itemToAdd = new List<GroupInfo>();
                    foreach (var cg in channelGroups)
                    {
                        foreach (var sp in stopGroups)
                        {
                            if (sp.GroupId == cg.GroupId)
                            {
                                if (!itemToAdd.Contains(cg))
                                {
                                    itemToAdd.Add(cg);
                                }
                            }
                        }

                    }


                    channelGroups = itemToAdd;



                }
                else
                {
                    List<GroupInfo> itemToAdd = new List<GroupInfo>();
                    if (GlobalUtils.GlobalGroupBusinessStatus.Count > 0)
                    {

                        foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                        {
                            if (grr.TypeRunning == BusinessTypeForGroup.ManualScheduleTask && grr.channelId == channelId)
                            {
                                foreach (var cg in channelGroups)
                                {
                                    if (cg.GroupId == grr.GroupId)
                                    {

                                        if (!itemToAdd.Contains(cg))
                                        {
                                            itemToAdd.Add(cg);
                                        }
                                    }

                                }

                            }
                        }

                    }

                    channelGroups = itemToAdd;
                }

                List<GroupInfo> giOut = new List<GroupInfo>();
                CreateCommandForStopRepeatToClients(cmdToVideoSvr.commandType, channelId, out clientsIpToSend, out clientsConectionIdToSend, out ipRealySent, out clientsDataToSend, bType, out giOut, channelGroups);


                QueueCommandType cmdType = QueueCommandType.NONE;

                if (isSchedule)
                {
                    cmdType = QueueCommandType.SCHEDULESTOP;

                }
                else
                {

                    cmdType = isWantToStop ? QueueCommandType.MANAULLYSTOP : QueueCommandType.MANAULLYREPEAT;
                }

                PushQueue(cmdType, clientsIpToSend, isSchedule, channelId, channelName, string.Empty, scheduleTime, cmdToVideoSvr.guidId, clientsDataToSend.guidId, isSendToVideoSvr);


                string str = "Stop Command Send BEFORE " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId;
                System.Diagnostics.Debug.WriteLine(str);
                GlobalUtils.WriteDebugLogs(str);

                string jsonDataToVideoSvr = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);

                if (!string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
                {
                    if (isSendToVideoSvr)
                    {
                        hub.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonDataToVideoSvr);
                        str = "STOP json To Video Sever: " + jsonDataToVideoSvr + DateTime.Now.ToString("HH:mm:ss fff");
                        System.Diagnostics.Debug.WriteLine(str);
                        GlobalUtils.WriteDebugLogs(str);
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



                    foreach (var css in ipRealySent)
                    {
                        PlayDevice itemToRemoved = null;
                        foreach (var pd in GlobalUtils.PlayingDevices)
                        {

                            if (css == pd.IpAddress)
                            {
                                itemToRemoved = pd;
                                break;
                            }
                        }

                        if (itemToRemoved != null)
                        {
                            GlobalUtils.PlayingDevices.Remove(itemToRemoved);
                        }
                    }

                    str = "STOP json To Adnriod Client: " + jsonDataToClient + DateTime.Now.ToString("HH:mm:ss fff");
                    System.Diagnostics.Debug.WriteLine(str);
                    GlobalUtils.WriteDebugLogs(str);
                }



                str = "Stop Command Send AFTER " + DateTime.Now.ToString("HH:mm:ss fff") + " Channel Id:" + channelId;
                System.Diagnostics.Debug.WriteLine(str);
                System.Diagnostics.Debug.WriteLine(str);
                new Thread(ProcessTimeOutRequest).Start(hub);

                if (!isStopClients)
                {

                    if (!isSchedule)
                    {
                        if (isWantToStop)
                        {

                            ManualPlayItem mp = GlobalUtils.GetManaulPlayItemByChannelId(channelId);


                            if (mp != null)
                            {
                                //   GlobalUtils.ChannelManuallyPlayingChannelId = string.Empty;
                                //   GlobalUtils.ChannelManuallyPlayingPids = null;
                                //  GlobalUtils.ChannelManuallyPlayingFunction = bType;


                                //    GlobalUtils.ChannelManuallyPlayingChannelName = string.Empty;
                                mp.IsPlaying = false;

                                mp.IsRepeating = false;




                                //    hub.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus("Stop", "0", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());
                                GlobalUtils.SendManuallyClientNotice(hub, "Stop", "0", mp);
                                GlobalUtils.AddLogs(hub, "手动操作", channelName + "手动停止成功");

                            }
                        }
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

                        ManualPlayItem mp = GlobalUtils.GetManaulPlayItemByChannelId(channelId);

                        if (mp != null && mp.IsPlaying)
                        {

                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = itemToRemove.GuidId + "," + itemToRemove.ChannelId;

                            cr.errorCode = "200";

                            cr.message = "计划停止失败，手工播放中";

                            if (!string.IsNullOrWhiteSpace(GlobalUtils.WindowsServiceConnectionId))
                            {

                                GlobalUtils.AddLogs(hub, "计划任务", channelName + "结束播放计划执行失败，手工播放中,运行时间：" + scheduleTime);
                                hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cr));
                            }

                            return;
                        }

                        if (itemToRemove != null)
                        {

                            ComuResponseBase cr = new ComuResponseBase();

                            cr.guidId = itemToRemove.GuidId + "," + itemToRemove.ChannelId;

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

                if (channelGroups != null && channelGroups.Count > 0)
                {
                    foreach (var c in channelGroups)
                    {
                        List<GroupBusinessRunning> itemToRemoved = new List<GroupBusinessRunning>();

                        foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                        {
                            if (grr.TypeRunning == BusinessTypeForGroup.ManualScheduleTask && grr.channelId == channelId && grr.GroupId == c.GroupId)
                            {
                                itemToRemoved.Add(grr);
                            }
                        }

                        foreach (var iv in itemToRemoved)
                        {
                            GlobalUtils.GlobalGroupBusinessStatus.Remove(iv);
                        }

                        GlobalUtils.AddLogs(hub, isSchedule ? "计划任务" : "手动操作", c.GroupName + "组停止成功");
                    }
                }

                List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

                hub.Clients(ids).sendRefreshAudioDeviceMessge();
            }

        }


        private static VideoOperAndriodClientCommand CreateAndroidCommandForStopRepeat(string commandType)
        {
            VideoOperAndriodClientCommand cmdToAndroidClient = new VideoOperAndriodClientCommand();

            cmdToAndroidClient.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


            cmdToAndroidClient.guidId = Guid.NewGuid().ToString();
            return cmdToAndroidClient;
        }


        private static void PushQueue(QueueCommandType cmdType, List<string> clientIps, bool isScheduled, string channelId, string channelName, string scheduleGuid, string scheduleTime, string severGuidId, string clientGuidId, bool isSendToVideoSvr = true, string newIpAddres = "",string currentVol="")
        {
            lock (GlobalUtils.ObjectLockQueueItem)
            {

                long currentTicks = DateTime.Now.Ticks;

                if (isSendToVideoSvr)
                {

                    GlobalUtils.CommandQueues.Add(new QueueItem() { ChannelId = channelId, ScheduleGuid = severGuidId, IsVideoServer = true, ScheduledTime = scheduleTime, ChannelName = channelName, IsScheduled = isScheduled, PushTicks = currentTicks, IpAddressStr = GlobalUtils.GetVideoServerConnectionIdentify(), GuidIdStr = severGuidId, CommandType = cmdType ,CurrentVol=currentVol});
                }

                //NO need send to client when is repeat operation.
                if (cmdType != QueueCommandType.MANAULLYREPEAT)
                {
                    foreach (var ip in clientIps)
                    {
                        GlobalUtils.CommandQueues.Add(new QueueItem() { NewAddressStr = newIpAddres, ChannelId = channelId, ScheduleGuid = severGuidId, ScheduledTime = scheduleTime, ChannelName = channelName, IsScheduled = isScheduled, PushTicks = currentTicks, IpAddressStr = ip, GuidIdStr = clientGuidId, CommandType = cmdType,CurrentVol=currentVol });
                    }
                }
            }
        }


        private static void CreatePlayCommandForAndriodClients(List<ProgramInfo> pids, VideoServerOperCommand cmdToVideoSvr, string channelId,
            out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out List<string> ipReallySent, out VideoOperAndriodClientCommand dataToSend, int maxBitRate, BusinessType bType, out List<GroupInfo> groups, List<GroupInfo> specGroups = null)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = CommandTypeEnum.PLAYVEDIO;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();



            dataSendToAndroidClient.arg = new VideoOperAndriodClientArg();


            if (pids != null && pids.Count > 0)
            {
                dataSendToAndroidClient.arg.bitRate = maxBitRate.ToString();

                dataSendToAndroidClient.arg.mediaType = GlobalUtils.CheckIfAudio(pids[0].MappingFiles[0].FileName) ? 1 : 2;

                dataSendToAndroidClient.arg.streamSrcs = cmdToVideoSvr.arg.streamSrcs;

                dataSendToAndroidClient.arg.udpBroadcastAddress = cmdToVideoSvr.arg.udpBroadcastAddress;
            }

            dataToSend = dataSendToAndroidClient;

            GetClientIpAndIdList(channelId, out ipsNeedToSend, out idsNeedToSend, out ipReallySent, bType, out groups, specGroups);
        }


        private static void CreateCommandForStopRepeatToClients(CommandTypeEnum cmdType, string channelId,
           out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out List<string> ipReallySent, out VideoOperAndriodClientCommand dataToSend, BusinessType bType, out List<GroupInfo> groups, List<GroupInfo> sepcGroups = null)
        {

            VideoOperAndriodClientCommand dataSendToAndroidClient = new VideoOperAndriodClientCommand();

            dataSendToAndroidClient.commandType = cmdType;


            dataSendToAndroidClient.guidId = Guid.NewGuid().ToString();



            dataToSend = dataSendToAndroidClient;



            GetClientIpAndIdList(channelId, out ipsNeedToSend, out idsNeedToSend, out ipReallySent, bType, out groups, sepcGroups);
        }

        private static void GetClientIpAndIdList(string channelId, out List<string> ipsNeedToSend, out List<string> idsNeedToSend, out List<string> ipReallySent, BusinessType bType, out List<GroupInfo> groupInfo, List<GroupInfo> specGroups = null)
        {
            List<GroupInfo> channelGroups;
            if (specGroups != null)
            {
                channelGroups = specGroups;
            }
            else
            {
                channelGroups = GlobalUtils.GroupBLLInstance.GetGroupByChannelId(channelId, bType);
            }

            groupInfo = channelGroups;
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
                idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.ANDROID, out ipReallySent);
            }
            else
            {
                ipReallySent = new List<string>();
                ipsNeedToSend = new List<string>();
                idsNeedToSend = new List<string>();
            }
        }



    }
}