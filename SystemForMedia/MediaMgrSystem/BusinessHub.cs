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
        public void SendStopRoRepeatCommand(string channelId, string channelName, bool isWantToStop, string scheduleGuidId)
        {
            SendLogic.SendStopRoRepeatCommand(channelId, channelName, isWantToStop, Clients, scheduleGuidId, "");

        }

        public void SendPlayCommand(string[] programeIds, string channelId, string channelName, string scheduleGuidId)
        {
            SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId, "");
        }


        public void SendMessageToMgrServer(string data, string connectionId)
        {

            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);

            lock (GlobalUtils.PublicObjectForLock)
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
                                strOperResult =  "视频服务器操作" + strOperResult;
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);                             

                            }
                            else
                            {
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                strOperResult ="终端" + matchIPAddress + "操作" + strOperResult;
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


        public void SendScheduleTaskControl(string channelId, string channelName, string[] pid, string cmdType, string guid, string scheduleTime)
        {
            //Play
            if (cmdType == "1")
            {
                SendLogic.SendPlayCommand(channelId, channelName, pid, Clients, guid, scheduleTime);

            }
            //Stop
            else if (cmdType == "2")
            {

                SendLogic.SendStopRoRepeatCommand(channelId, channelName, true, Clients, guid, scheduleTime);
            }
        }

    }
}