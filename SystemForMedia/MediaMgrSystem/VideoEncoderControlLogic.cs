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

    public static class VideoEncoderControlLogic
    {

        public static void SendVideoEncoderOperation(IHubCallerConnectionContext hub, string encoderId, bool isOpen, bool isLiveAudio)
        {


            lock (GlobalUtils.ObjectLockVideoEncoderQueueItem)
            {

                List<GroupInfo> groups = null;

                if (isLiveAudio)
                {
                    groups = GlobalUtils.GroupBLLInstance.GetGroupByVideoEncoderId(encoderId, BusinessType.AUDITBROADCAST);
                }
                else
                {
                    groups = GlobalUtils.GroupBLLInstance.GetGroupByVideoEncoderId(encoderId, BusinessType.VIDEOONLINE);
                }


                if (groups != null)
                {

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
                    if (needSentClientIpAddresses.Count > 0)
                    {
                        ipsNeedToSend = needSentClientIpAddresses;
                        List<string> ipReallySent = new List<string>();
                        idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.ANDROID, out ipReallySent);
                    }



                    VideoEncoderInfo vi = GlobalUtils.VideoEncoderBLLInstance.GetEncoderById(encoderId);


                    EncoderVideoOperCommand eor = new EncoderVideoOperCommand();

                    eor.commandType = isOpen ? CommandTypeEnum.VIDEOENCODEROPEN : CommandTypeEnum.VIDEOENCODERCLOSE;

                    VideoEncoderRunningItem itemToRemote = null; ;
                    for (int i = 0; i < GlobalUtils.RunningVideoEncoder.Count; i++)
                    {
                        VideoEncoderRunningItem rv = GlobalUtils.RunningVideoEncoder[i];
                        if (rv.EncoderId == encoderId)
                        {
                            itemToRemote = rv;

                        }
                    }

                    if (itemToRemote != null)
                    {
                        GlobalUtils.RunningVideoEncoder.Remove(itemToRemote);
                    }

                    if (isOpen)
                    {
                        BusinessTypeChecking outType;
                        if (!GlobalUtils.CheckIsGroupsRunningBusiness(groups, BusinessTypeChecking.VideoEncoder, out outType))
                        {
                            GlobalUtils.AddLogs(hub, "视频编码", GlobalUtils.CheckRunningBusinessTypeDesp(outType));
                            return;
                            //GlobalUtils.SendManuallyClientNotice(hub, GlobalUtils.CheckRunningBusinessTypeDesp(outType), "200", item);

                        }
                        VideoEncoderRunningItem item = new VideoEncoderRunningItem();
                        item.EncoderId = encoderId;
                        item.Groups = groups;
                        GlobalUtils.RunningVideoEncoder.Add(item);


                    }

                    eor.guidId = Guid.NewGuid().ToString();
                    eor.arg = new EncoderVideoOperationCommandeArg();
                    eor.arg.biteRate = vi.BaudRate;
                    eor.arg.mediaType = isLiveAudio ? "1" : "1";
                    eor.arg.udpBroadcastAddress = vi.UdpAddress;

                    if (groups != null && groups.Count > 0)
                    {
                        foreach (var c in groups)
                        {
                            if (isOpen)
                            {
                                GlobalUtils.AddLogs(hub, "视频编码", c.GroupName + "组播放成功");
                            }
                            else
                            {
                                GlobalUtils.AddLogs(hub, "视频编码", c.GroupName + "组停止成功");
                            }
                        }
                    }

                    PushQueue(isOpen ? QueueCommandType.VIDEOENCODEROPEN : QueueCommandType.VIDEOENCODEOCLOSE, ipsNeedToSend, eor.guidId, encoderId);

                    if (idsNeedToSend.Count > 0)
                    {

                        hub.Clients(idsNeedToSend).sendMessageToClient(Newtonsoft.Json.JsonConvert.SerializeObject(eor));

                    }

                    new Thread(ProcessTimeOutRequest).Start(hub);

                }


            }
        }


        private static void PushQueue(QueueCommandType cmdType, List<string> clientIps, string guidId, string encocodeId)
        {
            lock (GlobalUtils.ObjectLockVideoEncoderQueueItem)
            {

                long currentTicks = DateTime.Now.Ticks;


                foreach (var ip in clientIps)
                {
                    GlobalUtils.VideoEncoderQueues.Add(new VideoEncoderQueueItem { GuidIdStr = guidId, PushTicks = currentTicks, EncoderId = encocodeId, CommandType = cmdType, AndriodIpAddressStr = ip });
                }
            }
        }


        private static void ProcessTimeOutRequest(object hub)
        {
            try
            {
                Thread.Sleep(4000);

                lock (GlobalUtils.ObjectLockEncoderQueueItem)
                {
                    IHubConnectionContext hubContent = hub as IHubCallerConnectionContext;
                    List<VideoEncoderQueueItem> queueToRemoved = new List<VideoEncoderQueueItem>();
                    foreach (var que in GlobalUtils.VideoEncoderQueues)
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
                        foreach (VideoEncoderQueueItem item in queueToRemoved)
                        {

                            string ipToDisplay = string.Empty;

                            ipToDisplay = " 终端:" + item.AndriodIpAddressStr;

                            string strCmd = string.Empty;

                            strCmd = item.CommandType == QueueCommandType.VIDEOENCODEROPEN ? "打开视频源" : "关闭视频源";

                            GlobalUtils.AddLogs(hubContent, "视频源操作", strCmd + "," + ipToDisplay + "操作超时");


                            GlobalUtils.VideoEncoderQueues.Remove(item);
                            //  System.Diagnostics.Debug.WriteLine("Remove Command No Response: Now count is :" + GlobalUtils.CommandQueues.Count);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                GlobalUtils.AddLogs(null, "系统异常", ex.Message);
            }
        }


    }
}