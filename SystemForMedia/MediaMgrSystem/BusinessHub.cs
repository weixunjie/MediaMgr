using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using MediaMgrSystem.DataModels;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
namespace MediaMgrSystem
{
    [HubName("Test")]
    public class BusinessHub : Hub
    {

        public void sendVideoControlPauseMessage(string commandType)
        {
            Class2.SetCommand(2, Clients);
        }
        public void SendVideoControlMessage(string commandType)
        {

            Class2.SetCommand(1, Clients);

            // SendResponseMessage("sf");

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

            appendClient(time);

            WriteFileLine("IpAddress-" + IpAddress + " Server Sent Time: " + serverSendTime + "  Client Recieved Time: " + time);


        }

        private void WriteFileLine(string line)
        {
            StreamWriter sw = new StreamWriter(@"c:\log.txt", true);
            sw.WriteLine(line);

            sw.Close();



        }

        private void appendClient(string time)
        {
            StreamWriter sw = new StreamWriter(@"c:\logbb.txt", true);


            sw.WriteLine(time);

            sw.Close();


        }

        private List<string> getAllTime()
        {
            List<string> rsult = new List<string>();
            StreamReader sw = new StreamReader(@"c:\logbb.txt", false);
            while (!sw.EndOfStream)
            {

                string aa = sw.ReadLine();

                if (!string.IsNullOrEmpty(aa))
                {
                    rsult.Add(aa);
                }
            }
            sw.Close();

            return rsult;


        }

        public void SendSyncTimeResponse(string IpAddress, string time)
        {
            WriteFileLine("Now-" + IpAddress + " is " + time);

        }

    }
}