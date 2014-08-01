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
        public void SendStopRoRepeatCommand(string channelId, string channelName, bool isWantToStop, string scheduleGuidId, bool isRepeat)
        {
            GlobalUtils.ChannelManuallyPlayingIsRepeat = isRepeat;
            SendLogic.SendStopRoRepeatCommand(channelId, channelName, isWantToStop, Clients, scheduleGuidId, "");

        }

        public void SendPlayCommand(string[] programeIds, string channelId, string channelName, string scheduleGuidId, string isRepeat)
        {
            GlobalUtils.ChannelManuallyPlayingIsRepeat = false;
            SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "", isRepeat == "1");
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
                        if (rc.status != null && rc.status.Count > 0)
                        {
                            string strIP = GlobalUtils.GetIdentifyByConectionId(connectionId);
                            foreach (var sta in rc.status)
                            {
                                if (sta != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(sta.deviceType))
                                    {
                                        RemoveControlDeviceType type = RemoveControlDeviceType.NONE;
                                        if (Enum.TryParse(sta.deviceType, out type))
                                        {
                                            GlobalUtils.RemoteDeviceStatusBLLInstance.DeleteByClientIdentifyAndType(strIP, type);
                                            GlobalUtils.RemoteDeviceStatusBLLInstance.AddStatus(new RemoteDeviceStatus { ACMode = sta.acMode, ACTempature = sta.acTemperature, ClientIdentify = strIP, DeviceOpenedStatus = sta.isOpen == "1", DeviceType = type });
                                        }
                                    }


                                }
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



                                string strCmdType = RemoteControlLogic.GetCommandText(que.CommandType, que.ExternalIds);


                                GlobalUtils.AddLogs(Clients, "物联网控制", strCmdType + " " + strOperResult);


                                if (cb.errorCode == "0" && que.CommandType == QueueCommandType.REMOTECONTROLMANULCLOSE && que.CommandType == QueueCommandType.REMOTECONTROLMANULOPEN)
                                {
                                    //更新状态列表
                                    if (!string.IsNullOrWhiteSpace(que.ExternalIds))
                                    {
                                        string[] ids = que.ExternalIds.TrimEnd(',').Split(',');

                                        if (ids != null && ids.Length > 0)
                                        {
                                            for (int i = 0; i < ids.Length; i++)
                                            {
                                                string id = ids[i];
                                                if (!string.IsNullOrWhiteSpace(id))
                                                {

                                                    RemoveControlDeviceType type = RemoveControlDeviceType.NONE;
                                                    if (Enum.TryParse(id, out type))
                                                    {
                                                        GlobalUtils.RemoteDeviceStatusBLLInstance.DeleteByClientIdentifyAndType(matchIPAddress, type);
                                                        GlobalUtils.RemoteDeviceStatusBLLInstance.AddStatus(new RemoteDeviceStatus { ACMode = "", ACTempature = "", ClientIdentify = matchIPAddress, DeviceOpenedStatus = que.CommandType == QueueCommandType.REMOTECONTROLMANULOPEN, DeviceType = type });
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }

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

                            if (GlobalUtils.IsChannelManuallyPlaying)
                            {
                                if (GlobalUtils.ChannelManuallyPlayingChannelId == channelId)
                                {
                                    SendLogic.SendStopRoRepeatCommand(channelId, GlobalUtils.ChannelManuallyPlayingChannelName, true, Clients, "", "", false);

                                    Clients.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus("手工播放完毕停止", "1024");
                                }
                            }
                        }

                    }

                }

            }

            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);
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

                            }
                            else
                            {
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                strOperResult = "终端" + matchIPAddress + "操作" + strOperResult;
                            }


                            string strCmdType = GlobalUtils.GetCommandTextGetByType(que.CommandType);
                            if (que.IsScheduled)
                            {

                                GlobalUtils.AddLogs(Clients, "计划任务", que.ChannelName + strCmdType + strOperResult + "，运行时间：" + que.ScheduledTime);
                            }
                            else
                            {


                                GlobalUtils.AddLogs(Clients, "手动操作", que.ChannelName + strCmdType + strOperResult);
                            }

                            List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();



                            Clients.Clients(alPCIds).sendRefreshLogMessge(strOperResult, cb.errorCode);

                            removeGuid = cb.guidId;


                            break;
                        }
                    }
                }



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


        public void SendScheduleTaskControl(string channelId, string channelName, string[] pid, string cmdType, string guid, string scheduleTime, string isRepeat)
        {

            //object[] objs = new object[7];
            //objs[0] = channelId;
            //objs[1] = channelName;
            //objs[2] = pid;
            //objs[3] = guid;
            //objs[4] = scheduleTime;

            //objs[6] = isRepeat;

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
                SendLogic.SendPlayCommand(channelId, channelName, pid, Clients, guid, scheduleTime, isRepeat == "1");
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

                SendLogic.SendStopRoRepeatCommand(channelId, channelName, true, Clients, guid, scheduleTime);

                //  SendLogic.SendStopRoRepeatCommand(objs[0].ToString(), objs[1].ToString(), true, Clients, objs[3].ToString(), objs[4].ToString());

                // new Thread(ThreadToRunStopTask).Start(objs);


            }
        }

    }
}