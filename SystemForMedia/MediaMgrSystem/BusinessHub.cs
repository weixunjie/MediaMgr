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
        public void SendStopRoRepeatCommand(string commandType, string channelId,string scheduleGuidId)
        {
            SendLogic.SendStopRoRepeatCommand(commandType, Clients, scheduleGuidId, channelId);

        }

        public void SendPlayCommand(string channelId, string[] programeIds, string scheduleGuidId)
        {
            SendLogic.SendPlayCommand(channelId, programeIds, Clients, scheduleGuidId);
        }    
        
        public void sendVideoControlPauseMessage(string commandType)
        {
            HubSendLogic.SetCommand(2, Clients);
        }

        //public void SendVideoControlMessage(string commandType)
        //{

        //    //Clients.All.SendRefreshMessge("df");
        //    //return;


        //    //System.Diagnostics.Debug.WriteLine("Begin Send First Command " + DateTime.Now.ToString("HH:mm:ss.fff"));
        //    //HubSendLogic.SetCommand(1, Clients);
        //    //System.Diagnostics.Debug.WriteLine("End Send First Command " + DateTime.Now.ToString("HH:mm:ss.fff"));
        //    ////  var   aa= GlobalHost.TraceManager;

        //    //Thread.Sleep(4000);


        //    //System.Diagnostics.Debug.WriteLine("Begin Send 2nd Command " + DateTime.Now.ToString("HH:mm:ss.fff"));
        //    //HubSendLogic.SetCommand(2, Clients);
        //    //System.Diagnostics.Debug.WriteLine("End Send 2nd Command " + DateTime.Now.ToString("HH:mm:ss.fff"));


        //    ////   IPerformanceCounterManager 
        //    //     SendResponseMessage("sf");

        //}

        //public void SendScheduleTaskControl(string exTime, string stime)
        //{

        //    //HubSendLogic.SetCommand(1, Clients);

        //    //System.Diagnostics.Debug.WriteLine("WINDOWS SERVICE Schedule Execute At:" + exTime + "  Config Time:" + stime);


        //}


        public void SendMessageToMgrServer(string data, string ipAddress)
        {

            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);

            lock (GlobalUtils.PublicObjectForLock)
            {
                string removeIP = string.Empty; ;
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

                            if (ipAddress == GlobalUtils.VideoServerIPAddress)
                            {
                                str = que.CommandStr + " ->视频服务器操作" + strOperResult;

                            }
                            else
                            {
                                str = que.CommandStr + " ->终端（" + ipAddress + ")操作" + strOperResult;

                            }

                            List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                            Clients.Clients(alPCIds).sendResultBrowserClient(str, cb.errorCode);

                            removeIP = ipAddress;
                            removeGuid = cb.guidId;



                            break;
                        }
                    }
                }



                if (!string.IsNullOrEmpty(removeIP) && !string.IsNullOrEmpty(removeGuid))
                {
                    QueueItem removedItem = null;
                    foreach (var que in GlobalUtils.CommandQueues)
                    {
                        if (que.GuidIdStr == removeGuid && que.IpAddressStr == removeIP)
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


        public void sendScheduleTaskControl(string cid, string[] pid, string cmdType,string guid)
        {
            //Play
            if (cmdType == "1")
            {
                SendLogic.SendPlayCommand(cid, pid, Clients, guid);
                
            }
            //Stop
            else if (cmdType == "2")
            {

                SendLogic.SendStopRoRepeatCommand("1",Clients,guid,cid);
            }
        }

    }
}