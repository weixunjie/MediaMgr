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
    [HubName("Test")]
    public class BusinessHub : Hub
    {

        public IDependencyResolver Resolver { get; private set; }


        /// <summary>
        /// 1: stop
        /// 2: repeat
        /// </summary>
        /// <param name="commandType"></param>
        public void SendStopRoRepeatCommand(string commandType)
        {
            if (GlobalUtils.IsChannelPlaying)
            {
                string videoSvrId = GlobalUtils.GetVideoServerConnectionIds();

                if (string.IsNullOrWhiteSpace(videoSvrId))
                {
                    List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                    Clients.Clients(alPCIds).sendResultBrowserClient("视频服务未开启", "200");

                    Clients.Clients(alPCIds).sendResultBrowserClientNoticeStatus("视频服务未开启", "200");


                }

                else
                {
                    VideoServerOperCommand cmdToVideoSvr = new VideoServerOperCommand();

                    cmdToVideoSvr.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


                    GlobalUtils.CurrentVideoGuidId = Guid.NewGuid().ToString();

                    cmdToVideoSvr.guidId = GlobalUtils.CurrentVideoGuidId;




                    VideoOperAndriodClientCommand cmdToAndroidClient = CreateAndroidCommandForStopRepeat(commandType);


                    PushQueue(commandType == "1" ? "暂时" : "循环播放");

                    string jsonDataToVideoSvr = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
                    Clients.Client(videoSvrId).sendMessageToClient(jsonDataToVideoSvr);


                    string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToAndroidClient);

                    Clients.Clients(GlobalUtils.ReadyToSentClientIds).sendMessageToClient(jsonDataToClient);


                    if (commandType == "1")
                    {
                        GlobalUtils.IsChannelPlaying = false;
                    }

                }
            }

        }

        private VideoOperAndriodClientCommand CreateAndroidCommandForStopRepeat(string commandType)
        {
            VideoOperAndriodClientCommand cmdToAndroidClient = new VideoOperAndriodClientCommand();

            cmdToAndroidClient.commandType = commandType == "1" ? CommandTypeEnum.STOPVEDIO : CommandTypeEnum.REPEATPLAY;


            GlobalUtils.CurrentClientGuidId = Guid.NewGuid().ToString();

            cmdToAndroidClient.guidId = GlobalUtils.CurrentClientGuidId;
            return cmdToAndroidClient;
        }

        public void SendPlayCommand(string channelId, string[] programeIds)
        {
            if (GlobalUtils.IsChannelPlaying)
            {
                List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                Clients.Clients(alPCIds).sendResultBrowserClient("正在播放中", "200");
                return;
            }


            if (string.IsNullOrWhiteSpace(GlobalUtils.VideoServerConnectionId))
            {
                List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                Clients.Clients(alPCIds).sendResultBrowserClient("视频服务未开启", "200");
                Clients.Clients(alPCIds).sendResultBrowserClientNoticeStatus("视频服务未开启", "200");
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
                cmdToVideoSvr.arg.buffer = "5";
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

                cmdToVideoSvr.arg.udpBroadcastAddress = "udp://229.0.0.1:" + port.ToString();
                cmdToVideoSvr.arg.streamName = "123456790" + port.ToString();

                CreatePlayCommandForAndriodClients(pids, cmdToVideoSvr, channelId);

                PushQueue("播放视频");

                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(cmdToVideoSvr);
                Clients.Client(GlobalUtils.VideoServerConnectionId).sendMessageToClient(jsonData);



                string jsonDataToClient = Newtonsoft.Json.JsonConvert.SerializeObject(GlobalUtils.ReadyToSentClientData);
                Clients.Clients(GlobalUtils.ReadyToSentClientIds).sendMessageToClient(jsonDataToClient);

                GlobalUtils.IsChannelPlaying = true;
            }


        }

        private void PushQueue(string cmdText)
        {
            GlobalUtils.CommandQueues.Add(new QueueItem() { IpAddressStr = GlobalUtils.VideoServerIPAddress, GuidIdStr = GlobalUtils.CurrentVideoGuidId, CommandStr = cmdText });

            foreach (var ip in GlobalUtils.ReadyToSentClientIPs)
            {
                GlobalUtils.CommandQueues.Add(new QueueItem() { IpAddressStr = ip, GuidIdStr = GlobalUtils.CurrentClientGuidId, CommandStr = cmdText });
            }
        }


        private void CreatePlayCommandForAndriodClients(List<ProgramInfo> pids, VideoServerOperCommand cmdToVideoSvr, string channelId)
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
                QueueItem item2Removed = null;
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
                                item2Removed = que;
                            }
                            else if (ipAddress == que.IpAddressStr)
                            {
                                str = que.CommandStr + " ->终端（" + que.IpAddressStr + ")操作" + strOperResult;
                                item2Removed = que;
                            }


                            break;
                        }
                    }
                }

                if (item2Removed != null)
                {
                    List<String> alPCIds = GlobalUtils.GetAllPCDeviceConnectionIds();
                    Clients.Clients(alPCIds).sendResultBrowserClient(str, cb.errorCode);
                    GlobalUtils.CommandQueues.Remove(item2Removed);
                }

            }

        }

        public void SendTimeToServer(string aa)
        {
            //StreamWriter sw = new StreamWriter(@"c:\logForTrack.txt", true);
            //sw.WriteLine(aa);
            //sw.Close();
            lock (aa)
            {

                System.Diagnostics.Debug.WriteLine(aa);
            }

        }

        public void SendResponseMessage(string result)
        {
            VideoResponse vr = Newtonsoft.Json.JsonConvert.DeserializeObject<VideoResponse>(result);


            IList<string> ids = new List<string>();
            ids.Add(HttpContext.Current.Application["connectionId"].ToString());

            if (vr.errorCode == "0")
            {
                Clients.Clients(ids).sendResponseMessage(vr.deviceIP, " 成功");
            }
            else
            {
                Clients.Clients(ids).sendResponseMessage(vr.deviceIP, " 失败，原因：" + vr.message);
            }

        }

        public void SendPlayResponeMessage(string ipAddress)
        {

            System.Diagnostics.Debug.WriteLine("IP " + ipAddress + " Play Time:" + DateTime.Now.ToString("HH:mm:ss.fff"));

        }



        public void SendReceivedMessage(string IpAddress, string serverSendTime, string time)
        {

            DateTime dtServer = DateTime.Parse(serverSendTime);

            DateTime dtClient = DateTime.Parse(time);

            double ts = dtClient.Subtract(dtServer).TotalMilliseconds;



        }




        //private List<string> getAllTime()
        //{
        //    List<string> rsult = new List<string>();
        //    StreamReader sw = new StreamReader(@"c:\logbb.txt", false);
        //    while (!sw.EndOfStream)
        //    {

        //        string aa = sw.ReadLine();

        //        if (!string.IsNullOrEmpty(aa))
        //        {
        //            rsult.Add(aa);
        //        }
        //    }
        //    sw.Close();

        //    return rsult;


        //}

        public void SendSyncTimeResponse(string IpAddress, string time)
        {

        }

    }
}