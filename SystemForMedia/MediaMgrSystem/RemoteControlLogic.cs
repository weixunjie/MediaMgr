﻿using System;
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

    public static class RemoteControlLogic
    {

        public static void SendRemoteControlByGroups(IHubConnectionContext hub, string externalPointIds, string groupIds, bool isOpen, string acMode, string acTempure, string scheduleTime, string strWeeks, bool isSchduled)
        {

            lock (GlobalUtils.PublicObjectForLockRemoteControl)
            {

                if (!string.IsNullOrWhiteSpace(groupIds))
                {

                    groupIds = groupIds.TrimEnd(',');
                    List<GroupInfo> groups = GlobalUtils.GroupBLLInstance.GetGroupByIdAndBType(groupIds,BusinessType.REMOVECONTROL);

                    List<string> needSentClientIpAddresses = new List<string>();
                    if (groups != null && groups.Count > 0)
                    {
                        foreach (var gi in groups)
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

                    List<string> ipsNeedToSend = new List<string>();
                    List<string> idsNeedToSend = new List<string>();
                    List<string> ipReallySent = new List<string>();
                    if (needSentClientIpAddresses.Count > 0)
                    {
                        ipsNeedToSend = needSentClientIpAddresses;
                        idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.REMOTECONTORLDEVICE, out ipReallySent);
                    }


                    if (isSchduled)
                    {

                        string[] eid = externalPointIds.TrimEnd(',').Split(',');

                        if (eid != null && eid.Length > 0)
                        {
                            foreach (var str in eid)
                            {
                                foreach (var ip in ipsNeedToSend)
                                {
                                    GlobalUtils.RemoteDeviceScheduleTaskBLLInstance.DeleteByIdentifyAndType(ip, str, isOpen ? "1" : "0");

                                    RemoteDeviceScheduleTask task = new RemoteDeviceScheduleTask();
                                    task.ClientIdentify = ip;
                                    task.DeviceOpenedStatus = isOpen;
                                    task.DeviceType = str;
                                    task.ACMode = acMode;
                                    task.ACTempature = acTempure;
                                    task.TaskTime = scheduleTime;

                                    task.Weeks = strWeeks;

                                    GlobalUtils.RemoteDeviceScheduleTaskBLLInstance.AddTask(task);
                                }
                            }
                        }


                        //DeviceRemoteControlScheduleCommand cmd = new DeviceRemoteControlScheduleCommand();
                        //cmd.commandType = isOpen ? CommandTypeEnum.REMOTECONTRLSCHEDULEOPEN : CommandTypeEnum.REMOTECONTRLSCHEDULECLOSE;
                        //cmd.guidId = Guid.NewGuid().ToString();
                        //cmd.arg = new DeviceRemoteControlScheduleArg();
                        //externalPointIds = externalPointIds.TrimEnd(',');
                        //cmd.arg.devicesType = externalPointIds;
                        //cmd.arg.scheduleTime = scheduleTime;
                        //strWeeks = strWeeks.TrimEnd(',');
                        //cmd.arg.weekDays = strWeeks;
                        //cmd.arg.paramsData = new DeviceRemoteControlACParam();
                        //cmd.arg.paramsData.acMode = acMode;
                        //cmd.arg.paramsData.acTemperature = acTempure;
                        //QueueCommandType type = isOpen ? QueueCommandType.REMOTECONTROLSCHEDULEOPEN : QueueCommandType.REMOTECONTROLSCHEDULECLOSE;
                        //PushRemoteControlQueue(type, ipsNeedToSend, externalPointIds, cmd.guidId);

                        //if (idsNeedToSend.Count > 0)
                        //{

                        //    hub.Clients(idsNeedToSend).sendRemoteControlToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));

                        //}


                    }
                    else
                    {
                        string[] eid = externalPointIds.TrimEnd(',').Split(',');

                        if (eid != null && eid.Length > 0)
                        {
                            foreach (var str in eid)
                            {
                                foreach (var ip in ipsNeedToSend)
                                {
                                    SendRemoteControlBySingleDevice(hub, str, ip, isOpen, acMode, acTempure);
                                }
                            }
                        }

                        //DeviceRemoteControlManualCommand cmd = CreateManualControlCommand(externalPointIds, isOpen, acMode, acTempure);
                        //QueueCommandType type = isOpen ? QueueCommandType.REMOTECONTROLMANULOPEN : QueueCommandType.REMOTECONTROLMANULCLOSE;
                        //PushRemoteControlQueue(type, ipsNeedToSend, externalPointIds, cmd.guidId);

                        //if (idsNeedToSend.Count > 0)
                        //{

                        //    hub.Clients(idsNeedToSend).sendRemoteControlToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));

                        //}

                        //new Thread(ProcessTimeOutRequest).Start(hub);


                    }



                    // GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
                    // SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
                }
            }
        }

        public static void SendRemoteControlBySingleDevice(IHubConnectionContext hub, string externalPointId, string deviceIP, bool isOpen, string acMode, string acTempure)
        {

            lock (GlobalUtils.ObjectLockRemoteControlQueueItem)
            {

                DeviceRemoteControlManualCommand cmd = CreateManualControlCommand(externalPointId, isOpen, acMode, acTempure);

                List<string> ipReallySent = new List<string>();

                List<string> connectIds = GlobalUtils.GetConnectionIdsByIdentify(new List<string> { deviceIP }, SingalRClientConnectionType.REMOTECONTORLDEVICE, out ipReallySent);

                QueueCommandType type = isOpen ? QueueCommandType.REMOTECONTROLMANULOPEN : QueueCommandType.REMOTECONTROLMANULCLOSE;


                List<string> ips = new List<string>();

                ips.Add(deviceIP);

                PushRemoteControlQueue(type, ips, externalPointId, cmd.guidId);

                if (connectIds != null && connectIds.Count > 0)
                {

                    hub.Clients(connectIds).sendRemoteControlToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));
                }

                new Thread(ProcessTimeOutRequest).Start(hub);

            }
        }

        private static DeviceRemoteControlManualCommand CreateManualControlCommand(string currentExternalPointId, bool isOpen, string acMode, string acTempure)
        {
            DeviceRemoteControlManualCommand cmd = new DeviceRemoteControlManualCommand();
            cmd.commandType = isOpen ? CommandTypeEnum.REMOTECONTRLSCHEDULEOPEN : CommandTypeEnum.REMOTECONTRLSCHEDULECLOSE;
            cmd.guidId = Guid.NewGuid().ToString();
            cmd.arg = new DeviceRemoteControlManualArg();

            cmd.arg.devicesType = currentExternalPointId;

            cmd.arg.paramsData = new DeviceRemoteControlACParam();
            cmd.arg.paramsData.acMode = acMode;
            cmd.arg.paramsData.acTemperature = acTempure;



            return cmd;
        }

        private static void PushRemoteControlQueue(QueueCommandType cmdType, List<string> clientIps, string externalIds, string guidId)
        {

            long currentTicks = DateTime.Now.Ticks;


            foreach (var ip in clientIps)
            {
                GlobalUtils.RemoteControlCommandQueues.Add(new RemoteControlQueueItem() { ExternalIds = externalIds, IpAddressStr = ip, CommandType = cmdType, GuidIdStr = guidId, PushTicks = currentTicks });
            }

        }

        public static string GetCommandText(QueueCommandType type, string externalIds)
        {
            if (!string.IsNullOrEmpty(externalIds))
            {
                externalIds = externalIds.Replace("1", "空调");
                externalIds = externalIds.Replace("2", "电视");
                externalIds = externalIds.Replace("3", "投影仪");
                externalIds = externalIds.Replace("4", "电脑");
                externalIds = externalIds.Replace("5", "灯");

                externalIds = externalIds.TrimEnd(',');
                externalIds = externalIds.Replace(",", " ，");


            }

            switch (type)
            {

                case QueueCommandType.REMOTECONTROLMANULOPEN:
                    return "打开" + externalIds + "操作";

                case QueueCommandType.REMOTECONTROLMANULCLOSE:
                    return "关闭" + externalIds + "操作";
            }

            return string.Empty;
        }

        private static void ProcessTimeOutRequest(object hub)
        {
          
                Thread.Sleep(5000);

                lock (GlobalUtils.ObjectLockRemoteControlQueueItem)
                {
                    IHubConnectionContext hubContent = hub as IHubCallerConnectionContext;
                    List<RemoteControlQueueItem> queueToRemoved = new List<RemoteControlQueueItem>();
                    foreach (var que in GlobalUtils.RemoteControlCommandQueues)
                    {
                        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);

                        TimeSpan tsSubmited = new TimeSpan(que.PushTicks);
                        if (ts.Subtract(tsSubmited).Duration().TotalMilliseconds >= 5000)
                        {
                            queueToRemoved.Add(que);

                        }
                    }

                    if (queueToRemoved != null && queueToRemoved.Count > 0)
                    {
                        foreach (RemoteControlQueueItem item in queueToRemoved)
                        {

                            string ipToDisplay = string.Empty;

                            ipToDisplay = " 终端:" + item.IpAddressStr;



                            string strCmdType = GetCommandText(item.CommandType, item.ExternalIds);



                            GlobalUtils.AddLogs(hubContent, "物联网控制", strCmdType + " " + ipToDisplay + "操作超时");


                            GlobalUtils.RemoteControlCommandQueues.Remove(item);
                            //  System.Diagnostics.Debug.WriteLine("Remove Command No Response: Now count is :" + GlobalUtils.CommandQueues.Count);
                        }

                    }
                }
      



        }

    }
}