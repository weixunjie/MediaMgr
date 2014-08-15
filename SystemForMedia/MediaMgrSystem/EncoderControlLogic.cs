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

    public static class EncoderControlLogic
    {

        public static void SendEncoderOpenCommand(IHubCallerConnectionContext hub, string clientIdentify, string priority, string groupIds)
        {

            lock (GlobalUtils.ObjectLockEncoderQueueItem)
            {
                List<RunningEncoder> res = GlobalUtils.EncoderRunningClientsBLLInstance.GetAllEncoderRunning();

                if (res != null)
                {
                    foreach (var re in res)
                    {
                        if (re.ClientIdentify == clientIdentify)
                        {

                            GlobalUtils.AddLogs(hub, "呼叫台操作", clientIdentify + " 打开失败，正在运行中");

                            break;
                        }

                        if (int.Parse(re.Priority) > int.Parse(priority))
                        {
                            GlobalUtils.AddLogs(hub, "呼叫台操作", clientIdentify + " 打开失败，更高级别的呼叫台运行中");

                            break;
                        }
                    }
                }



            }

        }

        private static void PushRemoteControlQueue(QueueCommandType cmdType, List<string> clientIps, string groupIds, string guidId)
        {
            lock (GlobalUtils.ObjectLockRemoteControlQueueItem)
            {

                long currentTicks = DateTime.Now.Ticks;


                foreach (var ip in clientIps)
                {
                    GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { AndriodIpAddressStr = ip, GuidIdStr = guidId, GroupIds = groupIds, PushTicks = currentTicks, CommandType = cmdType });
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
                    List<EncoderQueueItem> queueToRemoved = new List<EncoderQueueItem>();
                    foreach (var que in GlobalUtils.EncoderQueues)
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
                        foreach (EncoderQueueItem item in queueToRemoved)
                        {

                            string ipToDisplay = string.Empty;

                            ipToDisplay = " 终端:" + item.AndriodIpAddressStr;

                            string strCmd = string.Empty;

                            strCmd = item.CommandType == QueueCommandType.ENCODEROPEN ? "打开呼叫台" : "关闭呼叫台";

                            GlobalUtils.AddLogs(hubContent, "呼叫台操作", strCmd + "," + ipToDisplay + "操作超时");


                            GlobalUtils.EncoderQueues.Remove(item);
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


        public static void SendEncoderOpenCommandToAndroid(IHubCallerConnectionContext hub, string clientIdentify, string priority, string groupIds)
        {

            lock (GlobalUtils.ObjectLockEncoderQueueItem)
            {
            
                if (!string.IsNullOrEmpty(groupIds))
                {
                    groupIds = groupIds.TrimEnd(',');
                    List<GroupInfo> groups = GlobalUtils.GroupBLLInstance.GetGroupByIds(groupIds);

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
                        idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.ANDROID);
                    }

                    EncoderInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(clientIdentify);

                    EncoderOpenCommand dopc=new EncoderOpenCommand();

                    dopc.commandType=CommandTypeEnum.ENCODEROPEN;

                    dopc.guidId=Guid.NewGuid().ToString();

                    dopc.arg = new EncoderOpenCommandeArg();

            
                    dopc.arg.baudRate = ei.BaudRate;
                    dopc.arg.streamName = "1234567890" + ei.EncoderId;
                    dopc.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;

              //      hub.Clients(idsNeedToSend).send

                    //{"guidId":"2847f884-a55b-4375-aca4-a7f2f2df08b9","commandType":"404"," message":””,"arg":{" udpBroadcastAddress":"udp://229.0.0.1:5000", “streamName”:”123789101”, baudRate  ”:”100”} } }

                    PushRemoteControlQueue(QueueCommandType.ENCODEROPEN, ipsNeedToSend, groupIds, dopc.guidId);
                }
            }

        }

    }
}