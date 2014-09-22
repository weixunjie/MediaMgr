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

    public static class VideoEncoderControlLogic
    {

        public static void SendVideoEncoderOperation(IHubCallerConnectionContext hub, string encoderId, bool isOpen)
        {



            lock (GlobalUtils.ObjectLockVideoEncoderQueueItem)
            {

                List<GroupInfo> groups = GlobalUtils.GroupBLLInstance.GetGroupByVideoEncoderId(encoderId, BusinessType.VIDEOONLINE);


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

                    eor.guidId = Guid.NewGuid().ToString();
                    eor.arg = new EncoderVideoOperationCommandeArg();
                    eor.arg.baudRate = vi.BaudRate;

                    eor.arg.udpBroadcastAddress = vi.EncoderId;


                    PushQueue(isOpen ? QueueCommandType.VIDEOENCODERAUDIOOPEN : QueueCommandType.VIDEOENCODERAUDIOCLOSE, ipsNeedToSend, eor.guidId, encoderId);

                    if (idsNeedToSend.Count > 0)
                    {

                        hub.Clients(idsNeedToSend).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(eor));

                    }

                    ProcessTimeOutRequest(hub);

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

                            strCmd = item.CommandType == QueueCommandType.VIDEOENCODERAUDIOOPEN ? "打开视频源" : "关闭视频源";

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