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
namespace MediaMgrSystem
{
    [HubName("Test")]
    public class BusinessHub : Hub
    {
        public IDependencyResolver Resolver { get; private set; }
        private object aa=new object ();

        public void LaoWeiTest()
        {
        
        }

        public void sendVideoControlPauseMessage(string commandType)
        {
            Class2.SetCommand(2, Clients);
        }
        public void SendVideoControlMessage(string commandType)
        {

            Clients.All.SendRefreshMessge("df");
            return;


            System.Diagnostics.Debug.WriteLine("Begin Send First Command "+ DateTime.Now.ToString("HH:mm:ss.fff"));
            Class2.SetCommand(1, Clients);
            System.Diagnostics.Debug.WriteLine("End Send First Command "+ DateTime.Now.ToString("HH:mm:ss.fff"));
       //  var   aa= GlobalHost.TraceManager;

             Thread.Sleep(4000);


             System.Diagnostics.Debug.WriteLine("Begin Send 2nd Command "+ DateTime.Now.ToString("HH:mm:ss.fff"));
             Class2.SetCommand(2, Clients);
             System.Diagnostics.Debug.WriteLine("End Send 2nd Command "+ DateTime.Now.ToString("HH:mm:ss.fff"));
          

         //   IPerformanceCounterManager 
        //     SendResponseMessage("sf");

        }

        public void SendScheduleTaskControl(string exTime, string stime)
        {

            Class2.SetCommand(1, Clients);

            System.Diagnostics.Debug.WriteLine("WINDOWS SERVICE Schedule Execute At:" + exTime + "  Config Time:" + stime);


        }



        public void SendMessageToMgrServer(string data)
        {
            VideoServerRegModel regModel = Newtonsoft.Json.JsonConvert.DeserializeObject<VideoServerRegModel>(data);

            if (regModel.commandType == CommandTypeEnum.VIDEOSVRREG)
            {
               GlobalUtils.UpdateConnectionByConnectionId(regModel.ConnectionId, SingalRClientConnectionType.VEDIOSERVER);
            }

        }
        public void SendTimeToServer(string aa)
        {
            //StreamWriter sw = new StreamWriter(@"c:\logForTrack.txt", true);
            //sw.WriteLine(aa);
            //sw.Close();
            lock(aa)
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