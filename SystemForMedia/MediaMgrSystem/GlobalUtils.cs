using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace MediaMgrSystem
{
    public enum QueueCommandType
    {
        NONE,
        MANAULLYPLAY,
        MANAULLYSTOP,
        MANAULLYREPEAT,
        SCHEDULEPLAY,
        SCHEDULESTOP,
        DEVICE_OPER_CHANGE_IP_ADDRESS = 128,

        DEVICE_OPER_OPENS_SCREEN = 122,
        DEVICE_OPER_CLOSE_SCREEN = 123,

        DEVICE_OPER_RESTART = 124,

        DEVICE_OPER_SHUTDOWN = 125,

        DEVICE_OPER_SCHEDULE_TURNON = 126,

        DEVICE_OPER_SCHEDULE_SHUTDOWN = 127,



        REMOTECONTROLMANULOPEN,
        REMOTECONTROLMANULCLOSE,
        REMOTECONTROLSCHEDULEOPEN,
        REMOTECONTROLSCHEDULECLOSE,

        ENCODEAUDIOROPEN,
        ENCODERAUDIOCLOSE
    }



    public class RemoteControlQueueItem
    {
        public string IpAddressStr
        {
            get;
            set;
        }

        public QueueCommandType CommandType
        { get; set; }

        public string GuidIdStr
        {
            get;
            set;
        }

        public string ExternalIds
        {
            get;
            set;
        }

        public long PushTicks
        {
            get;
            set;
        }

    }
    public class QueueItem
    {
        public string IpAddressStr
        { get; set; }

        public string NewAddressStr
        { get; set; }


        public QueueCommandType CommandType
        { get; set; }

        public string GuidIdStr
        {
            get;
            set;
        }

        public bool IsScheduled
        {
            get;
            set;
        }

        public long PushTicks
        {
            get;
            set;
        }

        public string ChannelName
        {
            get;
            set;
        }

        public string ScheduledTime
        {
            get;
            set;
        }

        public string ScheduleGuid
        {
            get;
            set;
        }

        public string ChannelId
        {
            get;
            set;
        }

        public bool IsVideoServer
        {
            get;
            set;
        }
    }




    public class EncoderQueueItem
    {
        public string AndriodIpAddressStr
        { get; set; }

        public QueueCommandType CommandType
        { get; set; }

        public string GuidIdStr
        {
            get;
            set;
        }

        public string EncoderClientIdentify
        {
            get;
            set;
        }

        public string EncoderPriority
        {
            get;
            set;
        }

        public string EncoderGroupIds
        {
            get;
            set;
        }

        public long PushTicks
        {
            get;
            set;
        }






    }

    public class ScheduleRunningItem
    {
        public string ChannelId
        { get; set; }

        public string GuidId
        { get; set; }

        public string RunningTime
        {
            get;
            set;
        }

        public string ChannelName
        {
            get;
            set;
        }
        public string Priority
        {
            get;
            set;
        }
    }

    public class ManualPlayItem
    {

        public string ChannelId { get; set; }
        public string ChannelName { get; set; }


        public string[] PlayingPids { get; set; }

        public bool IsPlaying { get; set; }

        public BusinessType PlayingFunction { get; set; }

        public bool IsRepeating { get; set; }

    }

    public static class GlobalUtils
    {

        public static object PublicObjectForLockRemoteControl = new object();

        public static object PublicObjectForLockRemoteControlRecievedMsg = new object();
        public static object PublicObjectForLockPlay = new object();

        public static object PublicObjectForLockStop = new object();

        public static object PublicObjectForLockClientMsg = new object();

        public static object LogForLock = new object();


        public static string StreamNameBase = "1234567890";
        public static object ObjectLockQueueItem = new object();
        public static object ObjectLockRemoteControlQueueItem = new object();
        public static object ObjectLockEncoderQueueItem = new object();
        private static object objForLock = new object();

        public static object ObjectLockForManualPlayItems = new object();

        public static DbUtils DbUtilsInstance = new DbUtils(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connString"].ToString());


        public static SingalConnectedClientsBLL SingalConnectedClientsBLLIntance = new SingalConnectedClientsBLL(GlobalUtils.DbUtilsInstance);

        public static GroupBLL GroupBLLInstance = new GroupBLL(GlobalUtils.DbUtilsInstance);

        public static RemoteDeviceStatusBLL RemoteDeviceStatusBLLInstance = new RemoteDeviceStatusBLL(GlobalUtils.DbUtilsInstance);


        public static List<RemoteDeviceStatus> AllRemoteDeviceStatus = new List<RemoteDeviceStatus>();
        public static ScheduleBLL ScheduleBLLInstance = new ScheduleBLL(DbUtilsInstance);

        public static ChannelBLL ChannelBLLInstance = new ChannelBLL(DbUtilsInstance);

        public static DeviceBLL DeviceBLLInstance = new DeviceBLL(DbUtilsInstance);

        public static ProgramBLL ProgramBLLInstance = new ProgramBLL(DbUtilsInstance);


        public static ParamConfigBLL ParamConfigBLLInstance = new ParamConfigBLL(DbUtilsInstance);
        public static UserBLL UserBLLInstance = new UserBLL(DbUtilsInstance);



        public static EncoderAudioBLL EncoderBLLInstance = new EncoderAudioBLL(DbUtilsInstance);

        public static EncoderAudioRunningClientsBLL EncoderAudioRunningClientsBLLInstance = new EncoderAudioRunningClientsBLL(DbUtilsInstance);

        public static LogBLL LogBLLInstance = new LogBLL(DbUtilsInstance);

        public static EncoderAudioRunningClientsBLL EncoderRunningClientsBLLInstance = new EncoderAudioRunningClientsBLL(DbUtilsInstance);



        public static FileInfoBLL FileInfoBLLInstance = new FileInfoBLL(DbUtilsInstance);



        public static List<ManualPlayItem> ManualPlayItems = new List<ManualPlayItem>();



        public static List<QueueItem> CommandQueues = new List<QueueItem>();


        public static List<RemoteControlQueueItem> RemoteControlCommandQueues = new List<RemoteControlQueueItem>();


        public static List<EncoderQueueItem> EncoderQueues = new List<EncoderQueueItem>();

        public static List<ScheduleRunningItem> RunningSchudules = new List<ScheduleRunningItem>();



        public static bool CheckIfAudio(string fileName)
        {
            string fName = fileName.ToUpper();

            return fName.EndsWith(".MP3");

        }

        public static ManualPlayItem GetManaulPlayItemByChannelId(string cid)
        {
            lock (ObjectLockForManualPlayItems)
            {
                foreach (var i in ManualPlayItems)
                {
                    if (cid == i.ChannelId)
                    {
                        return i;
                    }
                }

                return null;
            }

        }

        public static void AddManualPlayItem(ManualPlayItem mp)
        {
            lock (ObjectLockForManualPlayItems)
            {
                ManualPlayItem itemToRemove = null;
                foreach (var i in ManualPlayItems)
                {
                    if (mp.ChannelId == i.ChannelId)
                    {
                        itemToRemove = i;
                        break;
                    }
                }

                if (itemToRemove != null)
                {
                    ManualPlayItems.Remove(itemToRemove);
                }

                ManualPlayItems.Add(mp);
            }

        }

        public static void SetManaulPlayItemSatus(string cid, bool isPlay)
        {
            lock (ObjectLockForManualPlayItems)
            {
                for (int i = 0; i < ManualPlayItems.Count; i++)
                {
                    if (cid == ManualPlayItems[i].ChannelId)
                    {
                        ManualPlayItems[i].IsPlaying = isPlay;
                    }
                }
            }

        }

        public static void SetManaulPlayItemRepeat(string cid, bool isRepeat)
        {
            lock (ObjectLockForManualPlayItems)
            {
                for (int i = 0; i < ManualPlayItems.Count; i++)
                {
                    if (cid == ManualPlayItems[i].ChannelId)
                    {
                        ManualPlayItems[i].IsRepeating = isRepeat;
                    }
                }
            }

        }

        public static bool SetManaulPlayItemRepeatOffset(string cid)
        {
            lock (ObjectLockForManualPlayItems)
            {
                for (int i = 0; i < ManualPlayItems.Count; i++)
                {
                    if (cid == ManualPlayItems[i].ChannelId)
                    {
                        ManualPlayItems[i].IsRepeating = !ManualPlayItems[i].IsRepeating;

                        return ManualPlayItems[i].IsRepeating;
                    }
                }

                return false;
            }

        }




        public static bool RemoveConnectionByConnectionId(string connectionId)
        {
            lock (objForLock)
            {

                SingalConnectedClientsBLLIntance.DeleteSingalConnectedClientById(connectionId);

            }

            return true;

        }


        public static string CheckIfChannelManuallyPlayingFunctionIsCurrent(string channelId)
        {
            lock (ObjectLockForManualPlayItems)
            {

                ManualPlayItem mp = GetManaulPlayItemByChannelId(channelId);

                if (mp != null && mp.IsPlaying)
                {
                    if (GlobalUtils.GetCurrentFunctionType() == BusinessType.VIDEOONLINE)
                    {
                        return (mp.PlayingFunction == BusinessType.VIDEOONLINE).ToString().ToLower();
                    }
                    else
                    {
                        return (mp.PlayingFunction == BusinessType.AUDITBROADCAST).ToString().ToLower();
                    }
                }

                return "false";
            }
        }
        public static string WindowsServiceConnectionId
        {
            get
            {

                return GetWindowsServiceConnectionIds();
            }
        }


        public static string VideoServerConnectionId
        {
            get
            {

                return GetVideoServerConnectionIds();
            }
        }


        public static string GetVideoServerConnectionIds()
        {

            string result = string.Empty;

            lock (objForLock)
            {

                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByType(SingalRClientConnectionType.VEDIOSERVER.ToString());

                if (scs != null && scs.Count >= 1)
                {
                    result = scs[0].ConnectionId;
                }

            }

            return result;


        }

        public static string GetVideoServerConnectionIdentify()
        {

            string result = string.Empty;

            lock (objForLock)
            {

                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByType(SingalRClientConnectionType.VEDIOSERVER.ToString());

                if (scs != null && scs.Count >= 1)
                {
                    result = scs[0].ConnectionIdentify;
                }

            }

            return result;


        }

        public static string GetWindowsServiceConnectionIds()
        {


            string result = string.Empty;

            lock (objForLock)
            {

                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByType(SingalRClientConnectionType.WINDOWSSERVICE.ToString());

                if (scs != null && scs.Count >= 1)
                {
                    result = scs[0].ConnectionId;
                }

            }

            return result;

        }


        public static void AddConnection(SingalConnectedClient client)
        {
            lock (objForLock)
            {
                SingalConnectedClientsBLLIntance.AddSingalConnectedClient(client);
            }

        }


        //public static bool CheckIfVideoServer(string strIdentifies)
        //{
        //    bool result = false;

        //    lock (objForLock)
        //    {

        //        if (AllConnectedClients != null)
        //        {
        //            for (int i = 0; i < AllConnectedClients.Count; i++)
        //            {
        //                if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.VEDIOSERVER &&
        //                    strIdentifies == AllConnectedClients[i].ConnectionIdentify)
        //                {
        //                    result = true;
        //                    break;
        //                }
        //            }
        //        }

        //    }

        //    return result;

        //}

        public static string GetIdentifyByConectionId(string connId)
        {

            lock (objForLock)
            {

                SingalConnectedClient sc = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsById(connId);

                if (sc != null)
                {
                    return sc.ConnectionIdentify;
                }
            }

            return string.Empty;
        }

        public static BusinessType GetCurrentFunctionType()
        {

            if (HttpContext.Current.Session != null &&
                HttpContext.Current.Session["FunctionType"] != null &&
                HttpContext.Current.Session["FunctionType"].ToString() == "V")
            {

                return BusinessType.VIDEOONLINE;
            }


            return BusinessType.AUDITBROADCAST;
        }

        public static bool CheckIfPlaying()
        {
            foreach (var mp in ManualPlayItems)
            {
                if (mp.IsPlaying)
                {
                    return true;
                }
            }



            if (RunningSchudules != null && RunningSchudules.Count > 0)
            {
                return true;
            }



            return false;

        }

        public static List<string> GetConnectionIdsByIdentify(List<string> strIdentifies, SingalRClientConnectionType scType)
        {
            List<string> results = new List<string>();

            lock (objForLock)
            {

                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByIndetifies(strIdentifies);

                if (scs != null)
                {
                    foreach (var sc in scs)
                    {
                        if (sc != null)
                        {
                            if (sc.ConnectionType == scType)
                            {
                                results.Add(sc.ConnectionId);
                            }
                        }
                    }

                }

            }

            return results;

        }

        public static void SendManuallyClientNotice(IHubConnectionContext hub, string str, string errorCode,ManualPlayItem mp )
        {

            

                hub.Clients(GlobalUtils.GetAllPCDeviceConnectionIds()).sendManualPlayStatus(str, errorCode, mp.ChannelId, mp.ChannelName, mp.PlayingPids, mp.PlayingFunction==BusinessType.AUDITBROADCAST ? "1" : "2");
            
        }

        public static List<string> GetAllPCDeviceConnectionIds()
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {

                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByType(SingalRClientConnectionType.PC.ToString());

                if (scs != null)
                {
                    foreach (var sc in scs)
                    {
                        if (sc != null)
                        {
                            results.Add(sc.ConnectionId);
                        }
                    }

                }
            }


            return results;

        }


        public static bool CheckIfConnectionIdIsAndriod(string id)
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {

                SingalConnectedClient sc = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsById(id);


                if (sc != null)
                {
                    return sc.ConnectionType == SingalRClientConnectionType.ANDROID;
                }

            }


            return false;

        }

        public static bool CheckIfConnectionIdIsRemoteControlDevice(string id)
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {

                SingalConnectedClient sc = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsById(id);


                if (sc != null)
                {
                    return sc.ConnectionType == SingalRClientConnectionType.REMOTECONTORLDEVICE;
                }

            }


            return false;

        }

        private static void WriteErroLogs(string str)
        {
            StreamWriter sw = null;
            try
            {
                string logPath = @"d:\MediaSysMgrLogError.txt";
                if (!File.Exists(logPath))
                {

                    File.Create(logPath);
                }

                sw = new StreamWriter(logPath, true);

                sw.WriteLine(str);
            }
            catch
            {
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public static void WriteDebugLogs(string str)
        {
            // AddLogs(null, "调试日志", str);
        }



        public static void AddLogs(IHubConnectionContext hub, string logName, string logDesp)
        {

            try
            {
                LogBLLInstance.AddLog(logName, logDesp);

                List<String> alPCIds = GetAllPCDeviceConnectionIds();

                if (hub != null)
                {
                    hub.Clients(alPCIds).sendRefreshLogMessge();
                }
            }
            catch (Exception ex)
            {
                if (ex != null && !string.IsNullOrWhiteSpace(ex.Message))
                {

                    WriteErroLogs("程序异常" + ex.Message);
                }

            }

        }


        public static List<string> GetAllAndriodsDeviceConnectionIds()
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {
                List<SingalConnectedClient> scs = SingalConnectedClientsBLLIntance.GetSingalConnectedClientsByType(SingalRClientConnectionType.ANDROID.ToString());

                if (scs != null)
                {
                    foreach (var sc in scs)
                    {
                        if (sc != null)
                        {
                            results.Add(sc.ConnectionId);
                        }
                    }

                }


            }


            return results;

        }

        public static string GetCommandTextGetByType(QueueCommandType type)
        {
            switch (type)
            {
                case QueueCommandType.MANAULLYPLAY:
                    return "手工播放时";

                case QueueCommandType.MANAULLYSTOP:
                    return "手工结束播放时";

                case QueueCommandType.MANAULLYREPEAT:
                    return "手工循环播放时";

                case QueueCommandType.SCHEDULEPLAY:
                    return "播放计划运行时";

                case QueueCommandType.SCHEDULESTOP:
                    return "结束播放计划运行时";

                case QueueCommandType.REMOTECONTROLMANULCLOSE:
                    return "手工打开设备操作";

                case QueueCommandType.DEVICE_OPER_OPENS_SCREEN:
                    return "设备打开屏幕";
                case QueueCommandType.DEVICE_OPER_CLOSE_SCREEN:
                    return "设备关闭屏幕";

                case QueueCommandType.DEVICE_OPER_SHUTDOWN:
                    return "设备关机";

                case QueueCommandType.DEVICE_OPER_RESTART:
                    return "设备重起";

                case QueueCommandType.DEVICE_OPER_SCHEDULE_SHUTDOWN:
                    return "设备定时关机";
                case QueueCommandType.DEVICE_OPER_SCHEDULE_TURNON:
                    return "设备定时开机";

                case QueueCommandType.DEVICE_OPER_CHANGE_IP_ADDRESS:
                      return "修改IP地址";

                case QueueCommandType.ENCODEAUDIOROPEN:
                      return "打开音频编码时";

                case QueueCommandType.ENCODERAUDIOCLOSE:
                      return "关闭音频编码时";
            }

            return string.Empty;
        }
    }
}