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

    public static class CallerEncoderControlLogic
    {

        public class NeedStopManuaScheduleTask
        {
            public String channelId { get; set; }

            public String channelName { get; set; }



            public bool isSchedule { get; set; }

            public string scheduleTime { get; set; }


            public BusinessType bType { get; set; }

            public List<GroupInfo> groups { get; set; }
        }

        public class NeedStopVideoEncoderTask
        {
            public String encoderId { get; set; }


            public List<GroupInfo> groups { get; set; }
        }

        public static void SendEncoderAudioOpenCommand(IHubCallerConnectionContext hub, string clientIdentify, string priority, string groupIds, string devIds, bool isOperationFromDevice = false, string deviceReqeustGuiId = "")
        {

            try
            {
                 List<string > connecionIds = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetifyAll(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());

                lock (GlobalUtils.ObjectLockEncoderOperationItemOpen)
                {
                    if (connecionIds.Count<0)
                    {
                        GlobalUtils.AddLogs(hub, "呼叫台", "呼叫台设备未开启");
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

                                if (connecionIds.Count>0 && isOperationFromDevice)
                                {
                                    ComuResponseBase cb = new ComuResponseBase();
                                    cb.guidId = deviceReqeustGuiId;
                                    cb.errorCode = "110";
                                    cb.message = mesg;
                                    hub.Clients(connecionIds).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                                }

                                return;
                            }

                            if (int.Parse(re.Priority) > int.Parse(priority))
                            {
                                string msg = clientIdentify + " 打开失败，更高级别的呼叫台运行中";
                                GlobalUtils.AddLogs(hub, "呼叫台操作", msg);
                                if (connecionIds.Count > 0 && isOperationFromDevice)
                                {
                                    ComuResponseBase cb = new ComuResponseBase();
                                    cb.guidId = deviceReqeustGuiId;
                                    cb.errorCode = "110";
                                    cb.message = msg;


                                    hub.Clients(connecionIds).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                                }

                                return;
                            }
                        }
                    }

                    EncoderAudioInfo ei = GlobalUtils.EncoderBLLInstance.GetEncoderByClientIdentify(clientIdentify);



                    foreach (var r in res)
                    {
                        StopEncoder(hub, r.ClientIdentify, r.GroupIds, r.DevIds);

                    }

                    EncoderAudioOpenCommand eor = new EncoderAudioOpenCommand();

                    eor.guidId = Guid.NewGuid().ToString();
                    eor.arg = new EncoderAudioOpenCommandeArg();
                    eor.arg.baudRate = ei.BaudRate;
                    eor.arg.streamName = "1234567890" + ei.EncoderId;
                    eor.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;
                    eor.commandType = CommandTypeEnum.ENCODERAUDIOTOPEN;

                    GlobalUtils.EncoderAudioRunningClientsBLLInstance.UpdateRunningEncoder(new RunningEncoder() { ClientIdentify = clientIdentify, GroupIds = groupIds, DevIds = devIds, Priority = priority });




                    if (connecionIds.Count>0)
                    {


                        //      hub.Client(GlobalUtils.WindowsServiceConnectionId).sendMessageToWindowService(Newtonsoft.Json.JsonConvert.SerializeObject(cb));

                        EncoderAudioOpenReponse cb = new EncoderAudioOpenReponse();
                        cb.guidId = deviceReqeustGuiId;
                        cb.arg = new EncoderOpenReponseArg();

                        cb.arg.baudRate = ei.BaudRate;

                        cb.arg.streamName = eor.arg.streamName;

                        cb.arg.udpBroadcastAddress = eor.arg.udpBroadcastAddress;

                        string[] str = groupIds.Split(',');




                        List<GroupInfo> gis = new List<GroupInfo>();


                        if (str != null)
                        {
                            for (int i = 0; i < str.Length; i++)
                            {
                                gis.Add(GlobalUtils.GroupBLLInstance.GetGroupById(str[i])[0]);

                            }
                        }

                        List<GroupInfo> giToRemove = new List<GroupInfo>();

                        List<NeedStopVideoEncoderTask> needStopVideoEncoders = new List<NeedStopVideoEncoderTask>();


                        List<NeedStopManuaScheduleTask> needStopManaulMedias = new List<NeedStopManuaScheduleTask>();


                        if (groupIds != null)
                        {
                            String[] gid = groupIds.Split(',');
                            foreach (var c in gid)
                            {
                                GlobalUtils.GlobalGroupBusinessStatus.Add(new GroupBusinessRunning { GroupId = c, TypeRunning = BusinessTypeForGroup.AudioEncoder });

                                GlobalUtils.AddLogs(hub, "呼叫台操作", GlobalUtils.GroupBLLInstance.GetAllGroupsWithOutDeviceInfoByGroupId(c)[0].GroupName + "组呼叫成功");

                            }
                        }


                        List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

                        hub.Clients(ids).sendRefreshAudioDeviceMessge();

                        hub.Clients(ids).sendRefreshCallerEncoderDeviceMessge();

                        if (GlobalUtils.GlobalGroupBusinessStatus.Count > 0)
                        {
                            foreach (var gi in gis)
                            {

                                foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                                {
                                    if (grr.GroupId == gi.GroupId)
                                    {
                                        if (grr.TypeRunning == BusinessTypeForGroup.ManualScheduleTask)
                                        {
                                            //停止手工计划播放，


                                            bool isCidExisting = false;
                                            for (int i = 0; i < needStopManaulMedias.Count; i++)
                                            {
                                                if (needStopManaulMedias[i].channelId == grr.channelId)
                                                {
                                                    isCidExisting = true;
                                                    if (needStopManaulMedias[i].groups != null)
                                                    {
                                                        if (!needStopManaulMedias[i].groups.Contains(gi))
                                                        {
                                                            needStopManaulMedias[i].groups.Add(gi);
                                                        }
                                                    }
                                                }
                                            }

                                            if (!isCidExisting)
                                            {

                                                NeedStopManuaScheduleTask nst = new NeedStopManuaScheduleTask();
                                                nst.bType = grr.bType;
                                                nst.channelId = grr.channelId;
                                                nst.channelName = grr.channelName;
                                                nst.isSchedule = grr.isSchedule;
                                                nst.scheduleTime = grr.scheduleTime;
                                                nst.groups = new List<GroupInfo>();

                                                nst.groups.Add(gi);
                                                needStopManaulMedias.Add(nst);

                                            }






                                        }

                                        else if (grr.TypeRunning == BusinessTypeForGroup.VideoEncoder)
                                        {
                                            //停止视频编码

                                            bool isEncoderExsting = false;
                                            for (int i = 0; i < needStopVideoEncoders.Count; i++)
                                            {
                                                if (needStopVideoEncoders[i].encoderId == grr.encoderId)
                                                {
                                                    isEncoderExsting = true;
                                                    if (needStopVideoEncoders[i].groups != null)
                                                    {
                                                        if (!needStopVideoEncoders[i].groups.Contains(gi))
                                                        {
                                                            needStopVideoEncoders[i].groups.Add(gi);
                                                        }
                                                    }
                                                }
                                            }

                                            if (!isEncoderExsting)
                                            {

                                                NeedStopVideoEncoderTask nst = new NeedStopVideoEncoderTask();
                                                nst.encoderId = grr.encoderId;

                                                nst.groups = new List<GroupInfo>();

                                                nst.groups.Add(gi);
                                                needStopVideoEncoders.Add(nst);

                                            }

                                        }
                                    }
                                }


                            }

                            if (needStopVideoEncoders.Count > 0)
                            {
                                foreach (var task in needStopVideoEncoders)
                                {
                                    VideoEncoderControlLogic.SendVideoEncoderOperation(hub, task.encoderId, false, false, task.groups);
                                }
                                // VideoEncoderControlLogic.SendVideoEncoderOperation(hub, "", false, true, needStopVideoEncoders);
                            }

                            if (needStopManaulMedias.Count > 0)
                            {
                                foreach (var task in needStopManaulMedias)
                                {
                                    SendLogic.SendOutStopRepeatCommandToServerAndClient(task.channelId, task.channelName, true, hub, task.isSchedule, task.scheduleTime, task.bType, false, task.groups);
                                }

                            }
                        }


                        if (isOperationFromDevice)
                        {

                            if (str != null)
                            {
                                gis = new List<GroupInfo>();
                                for (int i = 0; i < str.Length; i++)
                                {
                                    gis.Add(new GroupInfo { GroupId = str[i] });

                                }
                            }



                            cb.errorCode = "0";
                            hub.Clients(connecionIds).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));



                        }
                        else
                        {
                            // GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { EncoderGroupIds = string.Empty, EncoderPriority = string.Empty, EncoderClientIdentify = clientIdentify, GuidIdStr = eor.guidId, CommandType = QueueCommandType.ENCODEAUDIOROPEN, PushTicks = DateTime.Now.Ticks });
                            hub.Clients(connecionIds).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(eor));
                        }

                        //     ProcessTimeOutRequest(hub);



                        //  SendCommandToAudioToEncoder(hub, clientIdentify, CommandTypeEnum.ENCODEROPEN);

                        Thread.Sleep(2000);

                        SendAudioEncoderCommandToAndroid(hub, clientIdentify, groupIds, devIds, false);





                    }
                }
            }

            catch (Exception ex)
            {
                try
                {



                    GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);


                }
                catch { }


                // HttpContext.Current.Response.Write(ex.StackTrace);
            }


        }

        private static void StopEncoder(IHubCallerConnectionContext hub, string clientIdentify, string groupIds, string devIds, bool isOperationFromDevice = false, string deviceReqeustGuiId = "")
        {

            if (groupIds != null)
            {
                String[] gid = groupIds.Split(',');


                List<GroupBusinessRunning> itemToRemoved = new List<GroupBusinessRunning>();

                foreach (var c in gid)
                {

                    foreach (var grr in GlobalUtils.GlobalGroupBusinessStatus)
                    {
                        if (grr.TypeRunning == BusinessTypeForGroup.AudioEncoder && grr.GroupId == c)
                        {
                            itemToRemoved.Add(grr);
                        }
                    }


                    GlobalUtils.AddLogs(hub, "呼叫台操作", GlobalUtils.GroupBLLInstance.GetAllGroupsWithOutDeviceInfoByGroupId(c)[0].GroupName + "组停止呼叫成功");

                }

                foreach (var iv in itemToRemoved)
                {
                    GlobalUtils.GlobalGroupBusinessStatus.Remove(iv);
                }
            }
            List<string> ids = GlobalUtils.GetAllPCDeviceConnectionIds();

            hub.Clients(ids).sendRefreshAudioDeviceMessge();


            hub.Clients(ids).sendRefreshCallerEncoderDeviceMessge();

            string connecionId = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetify(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());

            string strToSend = string.Empty;

            EncoderAudioCommandBase ec = new EncoderAudioCommandBase();
            ec.commandType = CommandTypeEnum.ENCODERAUDIOCLOSE;
            ec.guidId = Guid.NewGuid().ToString();
            strToSend = Newtonsoft.Json.JsonConvert.SerializeObject(ec);
            GlobalUtils.EncoderAudioRunningClientsBLLInstance.RemoveRunningEncoder(clientIdentify);



            if (!string.IsNullOrWhiteSpace(connecionId))
            {
                if (isOperationFromDevice)
                {
                    EncoderAudioOpenReponse cb = new EncoderAudioOpenReponse();
                    cb.guidId = deviceReqeustGuiId;
                    cb.arg = new EncoderOpenReponseArg();

                    cb.errorCode = "0";

                    hub.Client(connecionId).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));

                }
                else
                {
                    // GlobalUtils.EncoderQueues.Add(new EncoderQueueItem { EncoderGroupIds = string.Empty, EncoderPriority = string.Empty, EncoderClientIdentify = clientIdentify, GuidIdStr = ec.guidId, CommandType = QueueCommandType.ENCODERAUDIOCLOSE, PushTicks = DateTime.Now.Ticks });
                    hub.Client(connecionId).sendAudioEncoderCommandToClient(strToSend);
                }

            }

            SendAudioEncoderCommandToAndroid(hub, clientIdentify, groupIds, devIds, true);








        }

        public static void SendEncoderAudioCloseCommand(IHubCallerConnectionContext hub, string clientIdentify, bool isOperationFromDevice = false, string deviceReqeustGuiId = "")
        {
            try
            {

                lock (GlobalUtils.ObjectLockEncoderOperationItemClose)
                {
                    List<string> connecionIds = GlobalUtils.SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetifyAll(clientIdentify, SingalRClientConnectionType.ENCODERAUDIODEVICE.ToString());


                    if (connecionIds.Count<0&& isOperationFromDevice)
                    {
                        GlobalUtils.AddLogs(hub, "呼叫台操作", "呼叫台未开启");
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

                            if (connecionIds .Count>0&& isOperationFromDevice)
                            {
                                ComuResponseBase cb = new ComuResponseBase();
                                cb.guidId = deviceReqeustGuiId;
                                cb.errorCode = "110";
                                cb.message = msg;
                                hub.Clients(connecionIds).sendAudioEncoderCommandToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cb));
                            }

                            return;

                        }
                        else
                        {
                            StopEncoder(hub, isFound.ClientIdentify, isFound.GroupIds, isFound.DevIds, isOperationFromDevice, deviceReqeustGuiId);
                        }

                    }
                }
            }

            catch (Exception ex)
            {
                try
                {


                    GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);


                }
                catch { }


                // HttpContext.Current.Response.Write(ex.StackTrace);


            }
        }


        private static void PushCallerControlQueue(QueueCommandType cmdType, List<string> clientIps, string groupIds, string guidId)
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

                Thread.Sleep(5000);

                lock (GlobalUtils.ObjectLockEncoderQueueItem)
                {
                    IHubConnectionContext hubContent = hub as IHubCallerConnectionContext;
                    List<EncoderQueueItem> queueToRemoved = new List<EncoderQueueItem>();
                    foreach (var que in GlobalUtils.EncoderQueues)
                    {
                        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);

                        TimeSpan tsSubmited = new TimeSpan(que.PushTicks);
                        if (ts.Subtract(tsSubmited).Duration().TotalMilliseconds >= 5000)
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
                try
                {



                    GlobalUtils.AddLogs(null, "Exception", ex.StackTrace);


                }
                catch { }


                // HttpContext.Current.Response.Write(ex.StackTrace);
            }

        }


        public static void SendAudioEncoderCommandToAndroid(IHubCallerConnectionContext hub, string clientIdentify, string groupIds, string devIds, bool isStop)
        {


            List<string> needSentClientIpAddresses = new List<string>();
            if (!string.IsNullOrEmpty(groupIds))
            {
                groupIds = groupIds.TrimEnd(',');

                List<GroupInfo> groups = GlobalUtils.GroupBLLInstance.GetGroupByIds(groupIds, BusinessType.AUDITBROADCAST);

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
            }
            else
            {
                devIds = devIds.TrimEnd(',');

                String[] devs = devIds.Split(',');

                for (int i = 0; i < devs.Length; i++)
                {

                    List<DeviceInfo> dis = GlobalUtils.DeviceBLLInstance.GetADevicesById(devs[i]);

                    if (dis.Count > 0)
                    {
                        needSentClientIpAddresses.Add(dis[0].DeviceIpAddress);
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

            dopc.commandType = isStop ? CommandTypeEnum.ENCODERAUDIOCLOSE : CommandTypeEnum.ENCODERAUDIOTOPEN;

            dopc.guidId = Guid.NewGuid().ToString();

            if (!isStop)
            {
                dopc.arg = new EncoderAudioOpenCommandeArg();

                dopc.arg.baudRate = ei.BaudRate;
                dopc.arg.streamName = "1234567890" + ei.EncoderId;
                dopc.arg.udpBroadcastAddress = "udp://229.0.0.1:300" + ei.EncoderId;

            }

            if (idsNeedToSend.Count > 0)
            {
                hub.Clients(idsNeedToSend).sendMessageToClient(Newtonsoft.Json.JsonConvert.SerializeObject(dopc));

            }



            PushCallerControlQueue(isStop ? QueueCommandType.ENCODERAUDIOCLOSE : QueueCommandType.ENCODEAUDIOROPEN, ipsNeedToSend, groupIds, dopc.guidId);

            ProcessTimeOutRequest(hub);
        }




    }
}