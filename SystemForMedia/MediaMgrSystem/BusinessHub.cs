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
        public void SendStopRoRepeatCommand(string commandType, string channelId, string scheduleGuidId)
        {
            SendLogic.SendStopRoRepeatCommand(commandType, Clients, scheduleGuidId, channelId);

        }

        public void SendPlayCommand(string channelId, string channelName, string[] programeIds, string scheduleGuidId)
        {
            SendLogic.SendPlayCommand(channelId, channelName, programeIds, Clients, scheduleGuidId);
        }


        public void SendMessageToMgrServer(string data, string connectionId)
        {

            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);

            lock (GlobalUtils.PublicObjectForLock)
            {
                string matchIPAddress = string.Empty; ;
                String removeGuid = string.Empty;
                string str = string.Empty;
                foreach (var que in GlobalUtils.CommandQueues)
                {
                    if (cb != null && cb.errorCode != null)
                    {
                        if (que.GuidIdStr == cb.guidId)
                        {

                            string strOperResult = string.Empty;

                            strOperResult = cb.errorCode == "0" ? "成功" : "失败。错误消息编号" + cb.errorCode + ",内容：" + cb.message;

                            if (connectionId == GlobalUtils.VideoServerConnectionId)
                            {
                                str = que.CommandStr + " ->视频服务器操作" + strOperResult;

                            }
                            else
                            {
                                matchIPAddress = GlobalUtils.GetIdentifyByConectionId(connectionId);
                                str = que.CommandStr + " ->终端（" + matchIPAddress + ")操作" + strOperResult;

                            }

                            List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                            Clients.Clients(alPCIds).sendResultBrowserClient(str, cb.errorCode);
                       
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


        public void SendScheduleTaskControl(string cid, string[] pid, string cmdType, string guid)
        {
            //Play
            if (cmdType == "1")
            {
                SendLogic.SendPlayCommand(cid, string.Empty, pid, Clients, guid);

            }
            //Stop
            else if (cmdType == "2")
            {

                SendLogic.SendStopRoRepeatCommand("1", Clients, guid, cid);
            }
        }

    }
}