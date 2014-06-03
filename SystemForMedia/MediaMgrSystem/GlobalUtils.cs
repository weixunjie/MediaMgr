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
        public static List<SingalConnectedClient> andiordClients = new List<SingalConnectedClient>();

        private static string GetApplicationNameByType(SingalRClientConnectionType type)
        {

            switch (type)
            {
                case SingalRClientConnectionType.ANDROID:
                    return "ConnectedClientAndriods";
                case SingalRClientConnectionType.ENCODERDEVICE:
                    return "ConnectedClientEncoders";
                case SingalRClientConnectionType.PC:

                    return "ConnectedClientPCs";

                case SingalRClientConnectionType.VEDIOSERVER:
                    return "ConnectedClientVideoSever";

            }

            return string.Empty;

        }

        //public static bool RemoveConnectionByConnectionId(string connectionId)
        //{
        //    if (!RemoveConnectionByConnectionId(connectionId, GetApplicationNameByType(SingalRClientConnectionType.ANDROID)))
        //    {
        //        if (!RemoveConnectionByConnectionId(connectionId, GetApplicationNameByType(SingalRClientConnectionType.ENCODERDEVICE)))
        //        {
        //            if (!RemoveConnectionByConnectionId(connectionId, GetApplicationNameByType(SingalRClientConnectionType.PC)))
        //            {
        //                if (!RemoveConnectionByConnectionId(connectionId, GetApplicationNameByType(SingalRClientConnectionType.VEDIOSERVER)))
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    return true;
        //}

        public static bool UpdateConnectionByConnectionId(string connectionId, SingalRClientConnectionType type)
        {
            if (andiordClients != null)
            {
                int removeIndex = -1;
                for (int i = 0; i < andiordClients.Count; i++)
                {
                    if (andiordClients[i].ConnectionId == connectionId)
                    {
                        andiordClients[i].ConnectionType = type;
                        break;
                    }
                }


            }

            return true;
        }

        public static bool RemoveConnectionByConnectionId(string connectionId, string appName)
        {


            if (andiordClients != null)
            {
                int removeIndex = -1;
                for (int i = 0; i < andiordClients.Count; i++)
                {
                    if (andiordClients[i].ConnectionId == connectionId)
                    {
                        removeIndex = i;
                        break;
                    }
                }

                if (removeIndex >= 0)
                {
                    andiordClients.RemoveAt(removeIndex);


                }
            }

            return true;



            //HttpContext.Current.Application.Lock();
            //bool isRemoved = false;
            //if (HttpContext.Current.Application[appName] != null)
            //{
            //    List<SingalConnectedClient> connectedClients = (List<SingalConnectedClient>)HttpContext.Current.Application[appName];

            //    if (connectedClients != null)
            //    {
            //        int removeIndex = -1;
            //        for (int i = 0; i < connectedClients.Count; i++)
            //        {
            //            if (connectedClients[i].ConnectionId == connectionId)
            //            {
            //                removeIndex = i;
            //                break;
            //            }
            //        }

            //        if (removeIndex >= 0)
            //        {
            //            connectedClients.RemoveAt(removeIndex);
            //            isRemoved = true;

            //        }

            //        HttpContext.Current.Application[appName] = connectedClients;

            //    }
            //}

            //HttpContext.Current.Application.UnLock();
            //return isRemoved;


        }

        public static string GetVideoServerConnectionIds()
        {

            string result = string.Empty;

            if (andiordClients != null)
            {
                for (int i = 0; i < andiordClients.Count; i++)
                {
                    if (andiordClients[i].ConnectionType == SingalRClientConnectionType.VEDIOSERVER)
                    {
                        result = andiordClients[i].ConnectionId;
                    }
                }

            }

            return result;
            ////string result = string.Empty;
            ////HttpContext.Current.Application.Lock();

            ////string appName = GetApplicationNameByType(SingalRClientConnectionType.VEDIOSERVER);
            ////if (HttpContext.Current.Application[appName] != null)
            ////{
            ////    List<SingalConnectedClient> connectedClients = (List<SingalConnectedClient>)HttpContext.Current.Application[appName];

            ////    if (connectedClients != null)
            ////    {
            ////        for (int i = 0; i < connectedClients.Count; i++)
            ////        {
            ////            if (connectedClients[i].ConnectionType == SingalRClientConnectionType.VEDIOSERVER)
            ////            {
            ////                result = connectedClients[i].ConnectionId;
            ////                break;
            ////            }
            ////        }
            ////    }
            ////}

            ////HttpContext.Current.Application.UnLock();

            ////return result;
        }

        public static void AddConnection(SingalConnectedClient client)
        {
            andiordClients.Add(client);

            //HttpContext.Current.Appli
            // cation.Lock();

            //string appName = GetApplicationNameByType(client.ConnectionType);
            //if (HttpContext.Current.Application[appName] != null)
            //{
            //    List<SingalConnectedClient> connectedClients = (List<SingalConnectedClient>)HttpContext.Current.Application[appName];

            //    if (connectedClients != null)
            //    {
            //        connectedClients.Add(client);
            //        HttpContext.Current.Application[appName] = connectedClients;
            //    }
            //}
            //HttpContext.Current.Application.UnLock();

        }

        public static List<string> GetConnectionIdsByIdentify(string strIdentify, SingalRClientConnectionType type)
        {
            HttpContext.Current.Application.Lock();

            List<string> results = new List<string>();

            string appName = GetApplicationNameByType(type);

            if (HttpContext.Current.Application[appName] != null)
            {
                List<SingalConnectedClient> connectedClients = (List<SingalConnectedClient>)HttpContext.Current.Application[appName];

                if (connectedClients != null)
                {
                    for (int i = 0; i < connectedClients.Count; i++)
                    {
                        if (connectedClients[i].ConnectionIdentify == strIdentify)
                        {
                            results.Add(connectedClients[i].ConnectionIdentify);
                        }
                    }
                }
            }

            HttpContext.Current.Application.UnLock();
            return results;

        }

        public static List<string> GetAllAndriodsDeviceConnectionIds()
        {

            List<string> results = new List<string>();


            if (andiordClients != null)
            {
                for (int i = 0; i < andiordClients.Count; i++)
                {
                    if (andiordClients[i].ConnectionType == SingalRClientConnectionType.ANDROID)
                    {
                        results.Add(andiordClients[i].ConnectionId);
                    }
                }
            }


            return results;

        }
    }
}