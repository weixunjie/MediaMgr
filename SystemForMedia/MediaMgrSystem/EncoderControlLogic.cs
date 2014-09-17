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

    public static class EncoderAuditControlLogic
    {

        public static void SendEncoderAudioOpenCommand(IHubCallerConnectionContext hub, string clientIdentify, string priority, string groupIds, bool isOperationFromDevice = false, string deviceReqeustGuiId = "")
        {

            string connecionId = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());

            lock (GlobalUtils.ObjectLockEncoderQueueItem)
            {
                if (string.IsNullOrWhiteSpace(connecionId))
                {
                    GlobalUtils.AddLogs(hub, "音频编码", "音频编码设备未开启");
                    return;
                }


                List<RunningEncoder> res = GlobalUtils.EncoderRunningClientsBLLInstance.GetAllEncoderRunning();

                if (res != null)
                {
                    foreach (var re in res)
                    {
                        if (re.ClientIdentify == clientIdentify)
                        {

                            string mesg = clientIdentify + " 打开失败，正在运行中";
                            GlobalUtils.AddLogs(hub, "呼叫台操作", mesg);

                            if (!string.IsNullOrWhiteSpace(connecionId) && isOperationFromDevice)
                            {
                                ComuResponseBase cb = new ComuResponseBase();
                                cb.guidId = deviceReqeustGuiId;
                                cb.errorCode = "110";
                                cb.message = mesg;
                                hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                            }

                            return;
                        }

                        if (int.Parse(re.Priority) > int.Parse(priority))
                        {
                            string msg = clientIdentify + " 打开失败，更高级别的呼叫台运行中";
                            GlobalUtils.AddLogs(hub, "呼叫台操作", msg);
                            if (!string.IsNullOrWhiteSpace(connecionId) && isOperationFromDevice)
                            {
                                ComuResponseBase cb = new ComuResponseBase();
                                cb.guidId = deviceReqeustGuiId;
                                cb.errorCode = "110";
                                cb.message = msg;
                                hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                            }

                            return;
                        }
                    }
                }

                EncoderAudioInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(clientIdentify);



                foreach (var r in res)
                {
                    StopEncoder(hub, r.ClientIdentify, r.GroupIds);

                }

                EncoderAudioOpenCommand eor = new EncoderAudioOpenCommand();

                eor.guidId = Guid.NewGuid().ToString();
                eor.arg = new EncoderAudioOpenCommandeArg();
                eor.arg.baudRate = ei.BaudRate;
                eor.arg.streamName = "1234567890" + ei.EncoderId;
                eor.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;


                GlobalUtils.EncoderAudioRunningClientsBLLInstance.UpdateRunningEncoder(new RunningEncoder() { ClientIdentify = clientIdentify, GroupIds = groupIds, Priority = priority });




                if (!string.IsNullOrWhiteSpace(connecionId))
                {

                    GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { EncoderGroupIds = string.Empty, EncoderPriority = string.Empty, EncoderClientIdentify = clientIdentify, GuidIdStr = eor.guidId, CommandType = QueueCommandType.ENCODEAUDIOROPEN, PushTicks = DateTime.Now.Ticks });
                    //      hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cb));

                    if (isOperationFromDevice)
                    {
                        ComuResponseBase cb = new ComuResponseBase();
                        cb.guidId = deviceReqeustGuiId;
                        cb.errorCode = "0";
                        hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                    }
                    else
                    {
                        hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(ei));
                    }

                    ProcessTimeOutRequest(hub);


                    //  SendCommandToAudioToEncoder(hub, clientIdentify, CommandTypeEnum.ENCODEROPEN);

                    SendAudioEncoderCommandToAndroid(hub, clientIdentify, groupIds, false);

                }
            }

        }

        private static void StopEncoder(IHubCallerConnectionContext hub, string clientIdentify, string groupIds)
        {


            string connecionId = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());

            string strToSend = string.Empty;

            EncoderAudioCommandBase ec = new EncoderAudioCommandBase();
            ec.commandType = CommandTypeEnum.ENCODERCLOSE;
            ec.guidId = Guid.NewGuid().ToString();
            strToSend = Newtonsoft.Json.JsonConvert.SerializeObject(ec);
            GlobalUtils.EncoderAudioRunningClientsBLLInstance.RemoveRunningEncoder(clientIdentify);

            GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { EncoderGroupIds = string.Empty, EncoderPriority = string.Empty, EncoderClientIdentify = clientIdentify, GuidIdStr = ec.guidId, CommandType = QueueCommandType.ENCODERAUDIOCLOSE, PushTicks = DateTime.Now.Ticks });

            hub.Client(connecionId).sendAudioEncoderCommandToClient(strToSend);

            SendAudioEncoderCommandToAndroid(hub, clientIdentify, groupIds, true);

        }

        public static void SendEncoderAudioCloseCommand(IHubCallerConnectionContext hub, string clientIdentify, bool isOperationFromDevice = false, string deviceReqeustGuiId = "")
        {

            lock (GlobalUtils.ObjectLockEncoderQueueItem)
            {
                string connecionId = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());


                if (string.IsNullOrWhiteSpace(connecionId))
                {
                    GlobalUtils.AddLogs(hub, "音频编码", "音频编码设备未开启");
                    return;
                }

                List<RunningEncoder> res = GlobalUtils.EncoderRunningClientsBLLInstance.GetAllEncoderRunning();

                if (res != null)
                {
                    RunningEncoder isFound = null;
                    foreach (var re in res)
                    {

                        if (re.ClientIdentify == clientIdentify)
                        {
                            isFound = re;
                            break;
                        }

                    }

                    if (isFound == null)
                    {
                        string msg = clientIdentify + " 打开失败，呼叫台不在运行中";
                        GlobalUtils.AddLogs(hub, "呼叫台操作", msg);

                        if (!string.IsNullOrWhiteSpace(connecionId) && isOperationFromDevice)
                        {
                            ComuResponseBase cb = new ComuResponseBase();
                            cb.guidId = deviceReqeustGuiId;
                            cb.errorCode = "110";
                            cb.message = msg;
                            hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                        }

                        return;

                    }
                    else
                    {
                        StopEncoder(hub, isFound.ClientIdentify, isFound.GroupIds);
                    }

                }
            }

        }


        private static void PushRemoteControlQueue(QueueCommandType cmdType, List<string> clientIps, string groupIds, string guidId)
        {
            lock (GlobalUtils.ObjectLockEncoderQueueItem)
            {

                long currentTicks = DateTime.Now.Ticks;


                foreach (var ip in clientIps)
                {
                    GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { AndriodIpAddressStr = ip, GuidIdStr = guidId, PushTicks = currentTicks, CommandType = cmdType });
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

                            strCmd = item.CommandType == QueueCommandType.ENCODEAUDIOROPEN ? "打开呼叫台" : "关闭呼叫台";

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


        public static void SendAudioEncoderCommandToAndroid(IHubCallerConnectionContext hub, string clientIdentify, string groupIds, bool isStop)
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
                        List<string> ipReallySent = new List<string>();
                        idsNeedToSend = GlobalUtils.GetConnectionIdsByIdentify(needSentClientIpAddresses, SingalRClientConnectionType.ANDROID, out ipReallySent);
                    }

                    EncoderAudioInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(clientIdentify);

                    EncoderAudioOpenCommand dopc = new EncoderAudioOpenCommand();

                    dopc.commandType = isStop ? CommandTypeEnum.ENCODERCLOSE : CommandTypeEnum.ENCODEROPEN;

                    dopc.guidId = Guid.NewGuid().ToString();

                    if (!isStop)
                    {
                        dopc.arg = new EncoderAudioOpenCommandeArg();

                        dopc.arg.baudRate = ei.BaudRate;
                        dopc.arg.streamName = "1234567890" + ei.EncoderId;
                        dopc.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;

                    }
                    //      hub.Clients(idsNeedToSend).send

                    //{"guidId":"2847f884-a55b-4375-aca4-a7f2f2df08b9","commandType":"404"," message":””,"arg":{" udpBroadcastAddress":"udp://229.0.0.1:5000", “streamName”:”123789101”, baudRate  ”:”100”} } }

                    PushRemoteControlQueue(isStop ? QueueCommandType.ENCODERAUDIOCLOSE : QueueCommandType.ENCODEAUDIOROPEN, ipsNeedToSend, groupIds, dopc.guidId);

                    ProcessTimeOutRequest(hub);
                }
            }

        }

    }
}