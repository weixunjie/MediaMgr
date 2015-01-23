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
    [HubName("MediaMgrHub")]
    public class BusinessHub : Hub
    {

        public IDependencyResolver Resolver { get; private set; }


        /// <summary>
        /// 1: stop
        /// 2: repeat
        /// </summary>
        /// <param name="commandType"></param>
        public void SendStopRoRepeatCommand(string channelId, string channelName, bool isWantToStop, string scheduleGuidId, bool isRepeat, bool isAuditFun)
        {
            GlobalUtils.SetManaulPlayItemRepeat(channelId, isRepeat);
            SendLogic.SendStopRoRepeatCommand(channelId, channelName, isWantToStop, Clients, scheduleGuidId, "", isAuditFun ? BusinessType.AUDITBROADCAST : BusinessType.VIDEOONLINE);

        }

        public void SendChangeChannelSchedule(string cid, string channelName, string currentFunction)
        {
            SendLogic.SendChangeChannelSchedule(Clients, cid, channelName, currentFunction);
        }



        public void SendPlayCommand(string[] programeIds, string channelId, string channelName, string scheduleGuidId, string isRepeat, bool isAuditFun)
        {
            GlobalUtils.SetManaulPlayItemRepeat(channelId, false);
            SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1", isAuditFun ? BusinessType.AUDITBROADCAST : BusinessType.VIDEOONLINE);
        }

        public void SendDeviceOperCommand(string cmdStr, string groupId, string deviceIpAddress, string scheduleTurnOnTime, string scheduleShutDownTime, bool isEnabled, string volValue)
        {

            SendLogic.SendDeviceOperCommand(Clients, cmdStr, groupId, deviceIpAddress, scheduleTurnOnTime, scheduleShutDownTime, isEnabled ? 1 : 0, volValue);

            // GlobalUtils.SetManaulPlayItemRepeat(channelId, false);
            // SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1", isAuditFun ? BusinessType.AUDITBROADCAST : BusinessType.VIDEOONLINE);
        }


        public void RemoveDevice(string deviceIpAddress)
        {

            GlobalUtils.DeviceBLLInstance.RemoveDeviceByIpAddress(deviceIpAddress);

            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            Clients.Clients(ids).sendRefreshAudioDeviceMessge();

            //SendLogic.SendDeviceOperCommand(Clients, cmdStr, groupId, deviceIpAddress, scheduleTurnOnTime, scheduleShutDownTime, isEnabled ? 1 : 0, volValue);

            // GlobalUtils.SetManaulPlayItemRepeat(channelId, false);
            // SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1", isAuditFun ? BusinessType.AUDITBROADCAST : BusinessType.VIDEOONLINE);
        }


        public void SendAudioEncoderOpenCommand(string clientIdentify, string groupIds)
        {

            groupIds = groupIds.TrimEnd(',');

            EncoderAudioInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(clientIdentify);

            if (ei != null)
            {
                CallerEncoderControlLogic.SendEncoderAudioOpenCommand(Clients, clientIdentify, ei.Priority, groupIds, "");
            }


            // GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
            //SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
        }


        public void SendVideoEncoderOperation(string encoderId, string isOpen, string isLiveAudio)
        {
            VideoEncoderControlLogic.SendVideoEncoderOperation(Clients, encoderId, isOpen == "1", isLiveAudio == "1");
        }
        public void SendAudioEncoderCloseCommand(string clientIdentify)
        {
            CallerEncoderControlLogic.SendEncoderAudioCloseCommand(Clients, clientIdentify);

            // GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
            //SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
        }


        public void SendRemoteControlByGroups(string externalPointIds, string groupIds, bool isOpen, string acMode, string acTempure, string scheduleTime, string strWeeks, bool isSchduled)
        {
            RemoteControlLogic.SendRemoteControlByGroups(Clients, externalPointIds, groupIds, isOpen, acMode, acTempure, scheduleTime, strWeeks, isSchduled);
            // GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
            // SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
        }

        public void sendRemoteControlBySingleDevice(string externalPointId, string deviceIP, bool isOpen, string acMode, string acTempure)
        {

            RemoteControlLogic.SendRemoteControlBySingleDevice(Clients, externalPointId, deviceIP, isOpen, acMode, acTempure);

            // GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
            // SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
        }


        public void SendEncoderAudioControlToMgrServer(string data, string connectionId)
        {
            EncoderAudioOperationCommand rc = JsonConvert.DeserializeObject<EncoderAudioOperationCommand>(data);

            if (rc != null)
            {
                if (rc.commandType == CommandTypeEnum.ENCODERAUDIOTOPEN)
                {
                    EncoderAudioInfo eai = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(rc.clientIdentify);

                    //  string gids = string.Empty;

                    if (eai == null)
                    {
                        ComuResponseBase cbR = new ComuResponseBase();
                        cbR.guidId = rc.guidId;
                        cbR.errorCode = "150";
                        cbR.message = "";
                        Clients.Client(connectionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cbR));
                    }


                    if (!string.IsNullOrEmpty(rc.groupIds))
                    {
                        rc.groupIds = rc.groupIds.TrimEnd(',');

                    }

                    if (!string.IsNullOrEmpty(rc.devIds))
                    {
                        rc.devIds = rc.devIds.TrimEnd(',');

                    }



                    CallerEncoderControlLogic.SendEncoderAudioOpenCommand(Clients, rc.clientIdentify, eai.Priority.ToString(), rc.groupIds, rc.devIds, true, rc.guidId);

                }
                else if (rc.commandType == CommandTypeEnum.ENCODERAUDIOCLOSE)
                {
                    CallerEncoderControlLogic.SendEncoderAudioCloseCommand(Clients, rc.clientIdentify, true, rc.guidId);

                }
            }


            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);
            ProcessCallerQueu(cb, connectionId, true);
        }

        private void ProcessCallerQueu(ComuResponseBase cb, string connectionId, bool forCaller = true)
        {
            if (cb != null)
            {
                lock (GlobalUtils.ObjectLockEncoderQueueItem)
                {
                    string matchIPAddress = string.Empty; ;
                    String removeGuid = string.Empty;
                    string strOperResult = string.Empty;
                    foreach (var que in GlobalUtils.EncoderQueues)
                    {

                        if (cb != null && cb.errorCode != null)
                        {
                            if (que.GuidIdStr == cb.guidId)
                            {

                                strOperResult = cb.errorCode == "0" ? "成功" : "失败， 错误消息编号" + cb.errorCode + ",内容：" + cb.message;

                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                strOperResult = "终端" + matchIPAddress + "操作" + strOperResult;

                                removeGuid = cb.guidId;


                                string strCmdType = GlobalUtils.GetCommandTextGetByType(que.CommandType);

                                if (cb.errorCode != "0")
                                {
                                    GlobalUtils.AddLogs(Clients, "呼叫台操作", strCmdType + " " + strOperResult);
                                }






                                break;
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(matchIPAddress) && !string.IsNullOrEmpty(removeGuid))
                    {
                        EncoderQueueItem removedItem = null;
                        foreach (var que in GlobalUtils.EncoderQueues)
                        {
                            if (forCaller)
                            {
                                if (que.GuidIdStr == removeGuid && que.EncoderClientIdentify == matchIPAddress)
                                {
                                    removedItem = que;
                                    break;
                                }
                            }
                            else
                            {
                                if (que.GuidIdStr == removeGuid && que.AndriodIpAddressStr == matchIPAddress)
                                {
                                    removedItem = que;
                                    break;
                                }
                            }
                        }
                        if (removedItem != null)
                        {
                            GlobalUtils.EncoderQueues.Remove(removedItem);
                        }
                    }
                }



            }
        }

        public void SendRemoteControlToMgrServer(string data, string connectionId)
        {
            lock (GlobalUtils.PublicObjectForLockRemoteControlRecievedMsg)
            {
                System.Diagnostics.Debug.WriteLine(data + "  " + connectionId);



                ReceiveRemovoteDeivceCommand rc = JsonConvert.DeserializeObject<ReceiveRemovoteDeivceCommand>(data);

                if (rc != null || !string.IsNullOrWhiteSpace(rc.commandType.ToString()))
                {

                    if (rc != null && rc.commandType == CommandTypeEnum.REMOTECONTRLSENDSTATUS)
                    {

                        string strIP = GlobalUtils.GetIdentifyByConectionId(connectionId);


                        if (!string.IsNullOrWhiteSpace(rc.deviceType))
                        {
                            RemoveControlDeviceType type = RemoveControlDeviceType.NONE;
                            if (Enum.TryParse(rc.deviceType, out type))
                            {
                                GlobalUtils.RemoteDeviceStatusBLLInstance.DeleteByClientIdentifyAndType(strIP, type);
                                GlobalUtils.RemoteDeviceStatusBLLInstance.AddStatus(new RemoteDeviceStatus { ACMode = string.Empty, ACTempature = string.Empty, ClientIdentify = strIP, DeviceOpenedStatus = rc.state == "1", DeviceType = type });
                            }
                        }







                    }

                }

                ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);
                lock (GlobalUtils.RemoteControlCommandQueues)
                {
                    string matchIPAddress = string.Empty; ;
                    String removeGuid = string.Empty;
                    string strOperResult = string.Empty;
                    foreach (var que in GlobalUtils.RemoteControlCommandQueues)
                    {

                        if (cb != null && cb.errorCode != null)
                        {
                            if (que.GuidIdStr == cb.guidId)
                            {


                                strOperResult = cb.errorCode == "0" ? "成功" : "失败， 错误消息编号" + cb.errorCode + ",内容：" + cb.message;

                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                strOperResult = "终端" + matchIPAddress + "操作" + strOperResult;

                                removeGuid = cb.guidId;


                                string strCmdType = RemoteControlLogic.GetCommandText(que.CommandType, que.ExternalIds);
                                if (cb.errorCode != "0")
                                {

                                    GlobalUtils.AddLogs(Clients, "物联网控制", strCmdType + " " + strOperResult);

                                }
                                //if (cb.errorCode == "0" && que.CommandType == QueueCommandType.REMOTECONTROLMANULCLOSE && que.CommandType == QueueCommandType.REMOTECONTROLMANULOPEN)
                                //{
                                //    //更新状态列表
                                //    if (!string.IsNullOrWhiteSpace(que.ExternalIds))
                                //    {
                                //        string[] ids = que.ExternalIds.TrimEnd(',').Split(',');

                                //        if (ids != null && ids.Length > 0)
                                //        {
                                //            for (int i = 0; i < ids.Length; i++)
                                //            {
                                //                string id = ids[i];
                                //                if (!string.IsNullOrWhiteSpace(id))
                                //                {

                                //                    RemoveControlDeviceType type = RemoveControlDeviceType.NONE;
                                //                    if (Enum.TryParse(id, out type))
                                //                    {
                                //                        GlobalUtils.RemoteDeviceStatusBLLInstance.DeleteByClientIdentifyAndType(matchIPAddress, type);
                                //                        GlobalUtils.RemoteDeviceStatusBLLInstance.AddStatus(new RemoteDeviceStatus { ACMode = "", ACTempature = "", ClientIdentify = matchIPAddress, DeviceOpenedStatus = que.CommandType == QueueCommandType.REMOTECONTROLMANULOPEN, DeviceType = type });
                                //                    }
                                //                }
                                //            }
                                //        }
                                //    }

                                //}

                                break;
                            }
                        }
                    }



                    if (!string.IsNullOrEmpty(matchIPAddress) && !string.IsNullOrEmpty(removeGuid))
                    {
                        RemoteControlQueueItem removedItem = null;
                        foreach (var que in GlobalUtils.RemoteControlCommandQueues)
                        {
                            if (que.GuidIdStr == removeGuid && que.IpAddressStr == matchIPAddress)
                            {
                                removedItem = que;
                                break;
                            }
                        }
                        if (removedItem != null)
                        {
                            GlobalUtils.RemoteControlCommandQueues.Remove(removedItem);
                        }
                    }
                }

            }




        }

        public void SendAndroidLogsToMgrServer(string data)
        {

            LogsModel lm = JsonConvert.DeserializeObject<LogsModel>(data);

            foreach (var v in lm.values)
            {
                GlobalUtils.LogSignalRConnectionBLLBLLInstance.AddLog("AndroidNetWorkForClient", lm.ipAddress + "," + v.Replace("D", "断网").Replace("C", "连网"));
            }




        }

        public void SendCallerLogsToMgrServer(string data)
        {

            LogsModel lm = JsonConvert.DeserializeObject<LogsModel>(data);

            foreach (var v in lm.values)
            {
                GlobalUtils.LogSignalRConnectionBLLBLLInstance.AddLog("EncoderNetWorkForClient", lm.ipAddress + "," + v.Replace("D", "断网").Replace("C", "连网"));
            }


            if (!string.IsNullOrEmpty(lm.ipAddress))
            {
                RunningEncoder re = GlobalUtils.EncoderAudioRunningClientsBLLInstance.CheckIfEncoderRunning(lm.ipAddress);
                if (re != null && !string.IsNullOrEmpty(re.ClientIdentify))
                {

                    CallerEncoderControlLogic.SendEncoderAudioCloseCommand(Clients, lm.ipAddress);
                }
            }

        }
        public void SendDebugToMgrServer(string data, string connectionId)
        {
            Clients.Client(connectionId).sendDebugToClient(data);
        }

        public void SendMessageToMgrServer(string data, string connectionId)
        {

            System.Diagnostics.Debug.WriteLine(data + "  " + connectionId);
            System.Diagnostics.Debug.WriteLine("vid" + GlobalUtils.VideoServerConnectionId);



            ReceiveCommand rc = JsonConvert.DeserializeObject<ReceiveCommand>(data);

            if (rc != null || !string.IsNullOrWhiteSpace(rc.commandType.ToString()))
            {
                lock (GlobalUtils.ObjectLockQueueItem)
                {
                    if (rc != null && rc.commandType == CommandTypeEnum.STREAMSFINISHED)
                    {
                        if (rc.arg != null && !string.IsNullOrWhiteSpace(rc.arg.streamName))
                        {
                            string channelId = rc.arg.streamName.Replace(GlobalUtils.StreamNameBase, "");

                            if (rc.arg != null && !string.IsNullOrWhiteSpace(rc.arg.errorCode) && rc.arg.errorCode == "0")
                            {
                                ManualPlayItem mp = GlobalUtils.GetManaulPlayItemByChannelId(channelId);

                                if (mp != null && mp.IsPlaying)
                                {

                                    SendLogic.SendStopRoRepeatCommand(channelId, mp.ChannelName, true, Clients, "", "", mp.PlayingFunction, false, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent(channelId));


                                    GlobalUtils.SendManuallyClientNotice(Clients, "手工播放完毕停止", "1024", mp);

                                    // Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus("", "1024", channelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.ChannelManuallyPlayingFunction==BusinessType.AUDITBROADCAST?"1":"2");

                                }
                            }
                            else
                            {

                                ManualPlayItem mp = GlobalUtils.GetManaulPlayItemByChannelId(channelId);

                                if (mp != null && mp.IsPlaying)
                                {

                                    SendLogic.SendStopRoRepeatCommand(channelId, mp.ChannelName, true, Clients, "", "", mp.PlayingFunction, false);
                                    string errorStr = "手工播放停止,错误:";
                                    GlobalUtils.AddLogs(Clients, "手工播放", errorStr + rc.arg.message);
                                    //  Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus(errorStr + rc.arg.message, "1024", channelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids);

                                    GlobalUtils.SendManuallyClientNotice(Clients, errorStr + rc.arg.message, "1024", mp);
                                    // Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus("", "1024", channelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.ChannelManuallyPlayingFunction==BusinessType.AUDITBROADCAST?"1":"2");

                                }



                                lock (GlobalUtils.PublicObjectForLockPlay)
                                {
                                    if (GlobalUtils.RunningSchudules != null && GlobalUtils.RunningSchudules.Count > 0)
                                    {

                                        ScheduleRunningItem item = null;
                                        foreach (var s in GlobalUtils.RunningSchudules)
                                        {

                                            if (s.ChannelId == channelId)
                                            {

                                                item = s;
                                                break;


                                            }

                                        }

                                        if (item != null)
                                        {
                                            GlobalUtils.AddLogs(Clients, "计划任务", item.ChannelName + "计划失败，" + rc.arg.message + ", 运行时间：" + item.RunningTime);
                                            SendLogic.SendStopRoRepeatCommand(channelId, item.ChannelName, true, Clients, item.GuidId, item.RunningTime, BusinessType.AUDITBROADCAST, false);
                                        }


                                    }
                                }

                            }
                        }

                    }

                }

            }



            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);
            object[] objes = new object[2];
            objes[0] = cb;
            objes[1] = connectionId;
            new Thread(ProcessVideoEncoderReponse).Start(objes);
            new Thread(ProcessCallerEncoderReponse).Start(objes);

            lock (GlobalUtils.ObjectLockQueueItem)
            {
                string matchIPAddress = string.Empty; ;
                String removeGuid = string.Empty;
                string strOperResult = string.Empty;
                foreach (var que in GlobalUtils.CommandQueues)
                {

                    if (cb != null && cb.errorCode != null)
                    {
                        if (que.GuidIdStr == cb.guidId)
                        {
                            strOperResult = cb.errorCode == "0" ? "成功" : "失败， 错误消息编号" + cb.errorCode + ",内容：" + cb.message;

                            if (connectionId == GlobalUtils.VideoServerConnectionId)
                            {
                                strOperResult = "视频服务器操作" + strOperResult;
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);

                                if (cb.errorCode != "0")
                                {
                                    if (que.CommandType == QueueCommandType.MANAULLYPLAY)
                                    {
                                        ManualPlayItem mp = GlobalUtils.GetManaulPlayItemByChannelId(que.ChannelId);

                                        if (mp != null)
                                        {

                                            SendLogic.SendStopRoRepeatCommand(que.ChannelId, que.ChannelName, true, Clients, "", "", mp.PlayingFunction, false);
                                            string errorStr = "手工播放停止,错误:";
                                            GlobalUtils.AddLogs(Clients, "手工播放", errorStr + cb.message);

                                            GlobalUtils.SendManuallyClientNotice(Clients, errorStr + cb.message, "211", mp);

                                        }
                                        //Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus(errorStr + cb.message, "211", GlobalUtils.ChannelManuallyPlayingChannelId, GlobalUtils.ChannelManuallyPlayingChannelName, GlobalUtils.ChannelManuallyPlayingPids, GlobalUtils.CheckIfChannelManuallyPlayingFunctionIsCurrent());

                                    }
                                    else if (que.CommandType == QueueCommandType.SCHEDULEPLAY)
                                    {

                                        GlobalUtils.AddLogs(Clients, "计划任务", que.ChannelName + "播放计划失败，" + cb.message + ", 运行时间：" + que.ScheduledTime);

                                        SendLogic.SendStopRoRepeatCommand(que.ChannelId, que.ChannelName, true, Clients, que.ScheduleGuid, que.ScheduledTime, BusinessType.AUDITBROADCAST, false);
                                    }


                                }


                            }
                            else
                            {
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                strOperResult = "终端" + matchIPAddress + "操作" + strOperResult;
                            }


                            string strCmdType = GlobalUtils.GetCommandTextGetByType(que.CommandType);
                            //if (cb.errorCode != "0")
                            //{
                            //    if (que.IsScheduled)
                            //    {

                            //        GlobalUtils.AddLogs(Clients, "计划任务", que.ChannelName + strCmdType + strOperResult + "，运行时间：" + que.ScheduledTime);

                            //    }
                            //    else
                            //    {

                            //        GlobalUtils.AddLogs(Clients, "手动操作", que.ChannelName + strCmdType + strOperResult);
                            //    }
                            //}

                            if (cb.errorCode != "0" && que.CommandType == QueueCommandType.DEVICE_OPER_CHANGE_IP_ADDRESS)
                            {
                                DeviceInfo di = GlobalUtils.DeviceBLLInstance.GetADevicesByIPAddress(que.NewAddressStr)[0];

                                di.DeviceIpAddress = que.IpAddressStr;
                                GlobalUtils.DeviceBLLInstance.UpdateDevice(di);

                            }

                          

                            List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();


                            Clients.Clients(alPCIds).sendRefreshLogMessge(strOperResult, cb.errorCode);

                            removeGuid = cb.guidId;


                            break;
                        }
                    }
                }

                if (connectionId == GlobalUtils.VideoServerConnectionId)
                {
                    if (!string.IsNullOrEmpty(removeGuid))
                    {
                        QueueItem removedItem = null;
                        foreach (var que in GlobalUtils.CommandQueues)
                        {
                            if (que.GuidIdStr == removeGuid)
                            {
                                removedItem = que;
                                break;
                            }


                        }
                        if (removedItem != null)
                        {
                            GlobalUtils.CommandQueues.Remove(removedItem);


                            try
                            {
                                System.Diagnostics.Debug.WriteLine("kao" + "  " + GlobalUtils.CommandQueues[0].IpAddressStr + "->" + GlobalUtils.CommandQueues[0].GuidIdStr);
                            }
                            catch { }
                            // System.Diagnostics.Debug.WriteLine("vid" + GlobalUtils.VideoServerConnectionId);
                        }
                    }

                }
                else
                {

                    if (!string.IsNullOrEmpty(matchIPAddress) && !string.IsNullOrEmpty(removeGuid))
                    {
                        QueueItem removedItem = null;
                        foreach (var que in GlobalUtils.CommandQueues)
                        {
                            if (que.GuidIdStr == removeGuid && que.IpAddressStr == matchIPAddress)
                            {
                                removedItem = que;
                                break;
                            }


                        }
                        if (removedItem != null)
                        {
                            GlobalUtils.CommandQueues.Remove(removedItem);
                        }
                    }
                }
            }


        }

        private void ProcessCallerEncoderReponse(object cbObj)
        {
            object[] ojbs = cbObj as object[];


            ComuResponseBase cb = ojbs[0] as ComuResponseBase;
            string connectionId = ojbs[1] as string;
            ProcessCallerQueu(cb, connectionId, false);

        }
        private void ProcessVideoEncoderReponse(object cbObj)
        {
            object[] ojbs = cbObj as object[];


            ComuResponseBase cb = ojbs[0] as ComuResponseBase;
            string connectionId = ojbs[1] as string;
            lock (GlobalUtils.ObjectLockVideoEncoderQueueItem)
            {
                string matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId); ;
                String removeGuid = string.Empty;
                string strOperResult = string.Empty;
                foreach (var que in GlobalUtils.VideoEncoderQueues)
                {

                    if (cb != null && cb.errorCode != null)
                    {
                        if (que.GuidIdStr == cb.guidId && que.AndriodIpAddressStr == matchIPAddress)
                        {
                            strOperResult = cb.errorCode == "0" ? "成功" : "失败， 错误消息编号" + cb.errorCode + ",内容：" + cb.message;




                            string strCmd = que.CommandType == QueueCommandType.VIDEOENCODEROPEN ? "打开视频源" : "关闭视频源";
                            if (cb.errorCode != "0")
                            {
                                GlobalUtils.AddLogs(Clients, "视频源操作", strCmd + "," + que.AndriodIpAddressStr + "操作" + strOperResult);


                            }

                            matchIPAddress = que.AndriodIpAddressStr;

                            removeGuid = cb.guidId;


                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(matchIPAddress) && !string.IsNullOrEmpty(removeGuid))
                {
                    VideoEncoderQueueItem removedItem = null;
                    foreach (var que in GlobalUtils.VideoEncoderQueues)
                    {
                        if (que.GuidIdStr == removeGuid && que.AndriodIpAddressStr == matchIPAddress)
                        {
                            removedItem = que;
                            break;
                        }


                    }
                    if (removedItem != null)
                    {
                        GlobalUtils.VideoEncoderQueues.Remove(removedItem);
                    }
                }
            }
        }



        //private void ThreadToRunStartTask(object para)
        //{

        //    try
        //    {
        //        object[] objs = para as object[];

        //        string aa = "Play wait for... " + objs[5].ToString() + "->" + DateTime.Now.ToString("HH:mm:ss fff");
        //        System.Diagnostics.Debug.WriteLine(aa);
        //        GlobalUtils.WriteDebugLogs(aa);

        //        Thread.Sleep((int)objs[5]);



        //        SendLogic.SendPlayCommand(objs[0].ToString(), objs[1].ToString(), (string[])objs[2], Clients, objs[3].ToString(), objs[4].ToString(), (string)objs[6] == "1");

        //    }
        //    catch (Exception ex)
        //    {

        //        GlobalUtils.AddLogs(null, "程序异常:", ex.Message);

        //    }

        //}


        //private void ThreadToRunStopTask(object para)
        //{

        //    try
        //    {
        //        object[] objs = para as object[];

        //        string aa = "Stop wait for....  " + objs[5].ToString() + "->" + DateTime.Now.ToString("HH:mm:ss fff");
        //        System.Diagnostics.Debug.WriteLine(aa);
        //        GlobalUtils.WriteDebugLogs(aa);

        //        Thread.Sleep((int)objs[5]);

        //        SendLogic.SendStopRoRepeatCommand(objs[0].ToString(), objs[1].ToString(), true, Clients, objs[3].ToString(), objs[4].ToString());

        //    }
        //    catch (Exception ex)
        //    {

        //        GlobalUtils.AddLogs(null, "程序异常:", ex.Message);

        //    }

        //}


        //public void SendEncoderTaskControlCommandBack(string encoderResJson)
        //{
        //    ComuResponseBase cr = JsonConvert.DeserializeObject<ComuResponseBase>(encoderResJson);

        //    if (cr != null)
        //    {
        //        lock (GlobalUtils.ObjectLockEncoderQueueItem)
        //        {
        //            EncoderQueueItem itemToRemoved = null;
        //            foreach (var que in GlobalUtils.EncoderQueues)
        //            {
        //                if (que.GuidIdStr == cr.guidId)
        //                {
        //                    itemToRemoved = que;
        //                    break;
        //                }
        //            }

        //            if (cr.errorCode != "0")
        //            {
        //                string operStr = itemToRemoved.CommandType == QueueCommandType.ENCODEAUDIOROPEN ? "打开 " : " 关闭";
        //                GlobalUtils.AddLogs(Clients, "呼叫台操作", "呼叫台" + operStr + "失败，错误" + cr.message);
        //            }
        //            else
        //            {
        //                EncoderAuditControlLogic.SendEncoderCommandToAndroid(Clients, itemToRemoved.EncoderClientIdentify, itemToRemoved.EncoderGroupIds, itemToRemoved.CommandType == QueueCommandType.ENCODERAUDIOCLOSE);
        //            }

        //            if (itemToRemoved != null)
        //            {
        //                GlobalUtils.EncoderQueues.Remove(itemToRemoved);
        //            }
        //        }
        //    }

        //    // 

        //}


        //hubProxy.Invoke("sendRemoteControlTaskControl", strClientIdentify, strDeviceType, strNewDeviceStatus, strACTempature, strACMode);

        public void SendRemoteControlTaskControl(string strClientIdentify, string strDeviceType, string strNewDeviceStatus, string strACTempature, string strACMode)
        {
            RemoteControlLogic.SendRemoteControlBySingleDevice(Clients, strDeviceType, strClientIdentify, strNewDeviceStatus == "1", strACMode, strACTempature);
        }
        public void SendScheduleTaskControl(string channelId, string channelName, string[] pid, string cmdType, string guid, string scheduleTime, string isRepeat, string priority,string strIsForAudio)
        {

            //object[] objs = new object[7];
            //objs[0] = channelId;
            //objs[1] = channelName;
            //objs[2] = pid;
            //objs[3] = guid;
            //objs[4] = scheduleTime;

            //objs[6] = isRepeat;

            BusinessType bt = BusinessType.AUDITBROADCAST;

            if (!string.IsNullOrWhiteSpace(strIsForAudio) && strIsForAudio == "0")
            {
                bt = BusinessType.VIDEOONLINE;
            }

            //Play
            if (cmdType == "1")
            {
                //int intBufTime = 5000;
                //int nowTicks = DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

                //int schduleTicks = (DateTime.Now.Minute + 1) * 60000;


                //int offSet = schduleTicks - nowTicks;
                //if (offSet > intBufTime)
                //{
                //    int toSleep = offSet - intBufTime;

                //    objs[5] = toSleep;

                //}
                //else
                //{
                //    objs[5] = 0;

                //}

                string aa = "Play recveid and before to call send " + DateTime.Now.ToString("HH:mm:ss fff");
                System.Diagnostics.Debug.WriteLine(aa);
                GlobalUtils.WriteDebugLogs(aa);

                //   SendLogic.SendPlayCommand(objs[0].ToString(), objs[1].ToString(), (string[])objs[2], Clients, objs[3].ToString(), objs[4].ToString(), (string)objs[6] == "1");
            
                SendLogic.SendPlayCommand(channelId, channelName, pid, Clients, guid, scheduleTime, isRepeat == "1", bt, priority);
                // new Thread(ThreadToRunStartTask).Start(objs);

            }
            //Stop
            else if (cmdType == "2")
            {

                //int nowTicks = DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

                //int schduleTicks = (DateTime.Now.Minute + 1) * 60000;


                //int offSet = schduleTicks - nowTicks;
                //if (offSet > 0)
                //{

                //    objs[5] = offSet;

                //}
                //else
                //{
                //    objs[5] = 0;
                //}

                string aa = "Stop recveid and before to call send " + DateTime.Now.ToString("HH:mm:ss fff");
                System.Diagnostics.Debug.WriteLine(aa);
                GlobalUtils.WriteDebugLogs(aa);

                SendLogic.SendStopRoRepeatCommand(channelId, channelName, true, Clients, guid, scheduleTime, bt, true, priority);

                //  SendLogic.SendStopRoRepeatCommand(objs[0].ToString(), objs[1].ToString(), true, Clients, objs[3].ToString(), objs[4].ToString());

                // new Thread(ThreadToRunStopTask).Start(objs);


            }
        }

    }
}