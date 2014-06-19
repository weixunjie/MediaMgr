using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace MediaMgrSystem
{
    public class QueueItem
    {
        public string IpAddressStr
        { get; set; }

        public string CommandStr
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
    }

    public static class GlobalUtils
    {

        public static object PublicObjectForLock = new object();
        private static object objForLock = new object();
        public static DbUtils DbUtilsInstance = new DbUtils(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connString"].ToString());

        public static List<SingalConnectedClient> AllConnectedClients = new List<SingalConnectedClient>();


        public static GroupBLL GroupBLLInstance = new GroupBLL(GlobalUtils.DbUtilsInstance);


        public static ScheduleBLL ScheduleBLLInstance = new ScheduleBLL(GlobalUtils.DbUtilsInstance);

        public static ChannelBLL ChannelBLLInstance = new ChannelBLL(GlobalUtils.DbUtilsInstance);

        public static DeviceBLL DeviceBLLInstance = new DeviceBLL(GlobalUtils.DbUtilsInstance);

        public static ProgramBLL ProgramBLLInstance = new ProgramBLL(GlobalUtils.DbUtilsInstance);


        public static bool IsChannelManuallyPlaying = false;


        public static string[] ChannelManuallyPlayingPids = null;


        public static string ChannelManuallyPlayingChannelId = string.Empty;

        public static string ChannelManuallyPlayingChannelName = string.Empty;


        public static string CurrentVideoGuidId = string.Empty;

        public static string VideoServerIPAddress = string.Empty;

        public static string VideoServerConnectionId = string.Empty;

        public static string WindowsServiceConnectionId = string.Empty;


        public static string CurrentClientGuidId = string.Empty;

        public static object VideoSvrArg = null;

        //public static List<string> ReadyToSentClientIds = new List<string>();
        //public static List<string> ReadyToSentClientIPs = new List<string>();


        public static List<QueueItem> CommandQueues = new List<QueueItem>();

        //public static object ReadyToSentClientData = new object();

        //public static object ReadyToSentSVRData = new object();



        public static List<ScheduleRunningItem> RunningSchudules = new List<ScheduleRunningItem>();



        public static bool CheckIfAudio(string fileName)
        {
            string fName = fileName.ToUpper();

            if (fName.EndsWith(".FLV") || fName.EndsWith(".MP4"))
            {
                return false;
            }

            return true;

        }
        public static bool RemoveConnectionByConnectionId(string connectionId)
        {
            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    int removeIndex = -1;
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionId == connectionId)
                        {
                            removeIndex = i;
                            break;
                        }
                    }

                    if (removeIndex >= 0)
                    {
                        AllConnectedClients.RemoveAt(removeIndex);

                    }
                }
            }

            return true;

        }

        public static string GetVideoServerConnectionIds()
        {

            string result = string.Empty;

            lock (objForLock)
            {
                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.VEDIOSERVER)
                        {
                            result = AllConnectedClients[i].ConnectionId;
                        }
                    }
                }
            }

            return result;


        }

        public static string GetWindowsServiceConnectionIds()
        {

            string result = string.Empty;

            lock (objForLock)
            {
                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.WINDOWSSERVICE)
                        {
                            result = AllConnectedClients[i].ConnectionId;
                        }
                    }
                }
            }

            return result;


        }




        public static void AddConnection(SingalConnectedClient client)
        {
            lock (objForLock)
            {
                AllConnectedClients.Add(client);
            }

        }


        public static bool CheckIfVideoServer(string strIdentifies)
        {
            bool result = false;

            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.VEDIOSERVER &&
                            strIdentifies == AllConnectedClients[i].ConnectionIdentify)
                        {
                            result = true;
                            break;
                        }
                    }
                }

            }

            return result;

        }

        public static string  GetIdentifyByConectionId(string connId)
        {
            List<string> results = new List<string>();

            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionId==connId)
                        {
                            return AllConnectedClients[i].ConnectionIdentify;
                        }
                    }
                }
            }

            return "";
        }

        public static List<string> GetConnectionIdsByIdentify(List<string> strIdentifies)
        {
            List<string> results = new List<string>();

            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (strIdentifies.Contains(AllConnectedClients[i].ConnectionIdentify))
                        {
                            results.Add(AllConnectedClients[i].ConnectionId);
                        }
                    }
                }
            }

            return results;

        }


        public static List<string> GetAllPCDeviceConnectionIds()
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.PC)
                        {
                            results.Add(AllConnectedClients[i].ConnectionId);
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

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.ANDROID && AllConnectedClients[i].ConnectionId == id)
                        {
                            return true;
                        }
                    }
                }
            }


            return false;

        }

        public static List<string> GetAllAndriodsDeviceConnectionIds()
        {


            List<string> results = new List<string>();

            lock (objForLock)
            {

                if (AllConnectedClients != null)
                {
                    for (int i = 0; i < AllConnectedClients.Count; i++)
                    {
                        if (AllConnectedClients[i].ConnectionType == SingalRClientConnectionType.ANDROID)
                        {
                            results.Add(AllConnectedClients[i].ConnectionId);
                        }
                    }
                }
            }


            return results;

        }
    }
}