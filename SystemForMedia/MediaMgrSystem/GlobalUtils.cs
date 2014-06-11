using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace MediaMgrSystem
{
    public static class GlobalUtils
    {

        private static object objForLock = new object();
        public static DbUtils DbUtilsInstance = new DbUtils(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connString"].ToString());

        public static List<SingalConnectedClient> AllConnectedClients = new List<SingalConnectedClient>();


        public static GroupBLL GroupBLLInstance = new GroupBLL(GlobalUtils.DbUtilsInstance);

        public static ProgramBLL ProgramBLLInstance = new ProgramBLL(GlobalUtils.DbUtilsInstance);



        public static bool IsChannelWorking = false;

        public static string CurrentVideoGuidId = string.Empty;

        public static string CurrentClientGuidId = string.Empty;

        public static List<string> ReadyToSentClientIds = new List<string>();

        public static object ReadyToSentClientData = new object();


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
                            results.Add(AllConnectedClients[i].ConnectionIdentify);
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