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


        public void SendMessageToMgrServer(string data, string connectionId)
        {

           // System.Diagnostics.Debug.WriteLine(data + "  " + connectionId);
           // System.Diagnostics.Debug.WriteLine("vid" + GlobalUtils.VideoServerConnectionId);

            ComuResponseBase cb = JsonConvert.DeserializeObject<ComuResponseBase>(data);

            lock (GlobalUtils.PublicObjectForLock)
            {
                string matchIPAddress = string.Empty; ;
                String removeGuid = string.Empty;
                string strOperResult = string.Empty;
                foreach (var que in GlobalUtils.CommandQueues)
                {
                   // System.Diagnostics.Debug.WriteLine("CommandQueues " + que.GuidIdStr + " " + que.IpAddressStr + " " + que.ScheduledTime);
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

        private void ThreadToRunStartTask(object para)
        {

            object[] objs = para as object[];


            System.Diagnostics.Debug.WriteLine("Play wait for... " + objs[5].ToString() + "->" + DateTime.Now.ToString("HH:mm:ss S"));

            Thread.Sleep((int)objs[5]);


            SendLogic.SendPlayCommand(objs[0].ToString(), objs[1].ToString(), (string[])objs[2], Clients, objs[3].ToString(), objs[4].ToString(), (string)objs[6] == "1");


        }


        private void ThreadToRunStopTask(object para)
        {
            object[] objs = para as object[];

            System.Diagnostics.Debug.WriteLine("Stop wait for....  " + objs[5].ToString()+"->"+ DateTime.Now.ToString("HH:mm:ss S") );

            Thread.Sleep((int)objs[5]);
            

            SendLogic.SendStopRoRepeatCommand(objs[0].ToString(), objs[1].ToString(), true, Clients, objs[3].ToString(), objs[4].ToString());
        }


        public void SendScheduleTaskControl(string channelId, string channelName, string[] pid, string cmdType, string guid, string scheduleTime, string isRepeat)
        {

            object[] objs = new object[7];
            objs[0] = channelId;
            objs[1] = channelName;
            objs[2] = pid;
            objs[3] = guid;
            objs[4] = scheduleTime;

            objs[6] = isRepeat;

            //Play
            if (cmdType == "1")
            {
                int intBufTime = 5000;
                int nowTicks = DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

                int schduleTicks = (DateTime.Now.Minute + 1) * 60000;


                int offSet = schduleTicks - nowTicks;
                if (offSet > intBufTime)
                {
                    int toSleep = offSet - intBufTime;

                    objs[5] = toSleep;

                }
                else
                {
                    objs[5] = 0;

                }


                new Thread(ThreadToRunStartTask).Start(objs);

            }
            //Stop
            else if (cmdType == "2")
            {

                int nowTicks = DateTime.Now.Minute * 60000 + DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;

                int schduleTicks = (DateTime.Now.Minute + 1) * 60000;


                int offSet = schduleTicks - nowTicks;
                if (offSet > 0)
                {

                    objs[5] = offSet;

                }
                else
                {
                    objs[5] = 0;
                }


                new Thread(ThreadToRunStopTask).Start(objs);


            }
        }

    }
}