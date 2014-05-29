using MediaMgrSystem.DataModels;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace MediaMgrSystem
{
    public static class Class2
    {
        public static void SetCommand(int cd, IHubConnectionContext cs)
        {
            
            VideoOperCommand vo = new VideoOperCommand();
            vo.arg = new VideoOperArg();
            vo.arg.broadcastFlag = "1";

            if (cd == 1)
            {
                vo.commandType = CommandTypeEnum.PLAYVEDIO;
            }
            else if (cd == 2)
            {
                vo.commandType = CommandTypeEnum.STOPVEDIO;
            }

            vo.arg.destDeviceIP = "";
            vo.guidId = Guid.NewGuid().ToString();
            vo.arg.streamSrc = "test.mp4";
            vo.arg.bitRate = "1";
            vo.arg.mediaType = 1;
            vo.arg.udpBroadcastAddress = "udp://229.0.0.1:1234";

            vo.arg.streamName = Guid.NewGuid().ToString();

            //if (cd == 2)
            //{

            //    Clients.All.sendAllMessge(Newtonsoft.Json.JsonConvert.SerializeObject(vo));
            //    return;
            //}

            VideoServerOperCommand voc = new VideoServerOperCommand();

            if (cd == 1)
            {
                voc.commandType = CommandTypeEnum.PLAYVEDIO;
            }
            else if (cd == 2)
            {
                voc.commandType = CommandTypeEnum.STOPVEDIO;
            }

            voc.arg = new VideoServerOperArg();
            voc.arg.currentTime = DateTime.Now.ToString("HH:mm:ss");

            voc.guidId = vo.guidId;
            voc.arg.buffer = "5";
            voc.arg.streamName = "123456790";
            voc.arg.streamSrc = vo.arg.streamSrc;

            voc.arg.udpBroadcastAddress = vo.arg.udpBroadcastAddress;

            System.Diagnostics.Debug.WriteLine("Begin Send to Client " + DateTime.Now.ToString("HH:mm:ss.fff"));




            string videoServerId = GlobalUtils.GetVideoServerConnectionIds();


            if (!string.IsNullOrWhiteSpace(videoServerId))
            {
                cs.AllExcept(videoServerId).sendAllMessge(Newtonsoft.Json.JsonConvert.SerializeObject(vo));

                System.Diagnostics.Debug.WriteLine("End Send to Client " + DateTime.Now.ToString("HH:mm:ss.fff"));

                System.Diagnostics.Debug.WriteLine("Begin Send to Server " + DateTime.Now.ToString("HH:mm:ss.fff"));
                String str = Newtonsoft.Json.JsonConvert.SerializeObject(voc);


                cs.Client(videoServerId).sendMessageToClient(str);

                System.Diagnostics.Debug.WriteLine("End Send to Server " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
            //System.Diagnostics.Debug.WriteLine("Send to Server Done Json Serizalation" + DateTime.Now.ToString("HH:mm:ss.fff"));

            //System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["VideoUrl"].ToString() + "?PushTask=" + str);


            //httpWebRequest.KeepAlive = true;

            //System.Diagnostics.Debug.WriteLine("Send to Server Done Init web request" + DateTime.Now.ToString("HH:mm:ss.fff"));


            //httpWebRequest.ContentType = "application/json; charset=utf-8";

            //var response = httpWebRequest.GetResponse();

            //System.Diagnostics.Debug.WriteLine("Send to Server Done Get Response" + DateTime.Now.ToString("HH:mm:ss.fff"));

            //Stream streamResponse = response.GetResponseStream();

            //System.Diagnostics.Debug.WriteLine("Send to Server Done Get Stream" + DateTime.Now.ToString("HH:mm:ss.fff"));

            //StreamReader streamRead = new StreamReader(streamResponse);

            //String responseString = streamRead.ReadToEnd();
            //System.Diagnostics.Debug.WriteLine("Send to Server Done Convert Stream 2 String" + DateTime.Now.ToString("HH:mm:ss.fff"));
            //System.Diagnostics.Debug.WriteLine("End Send to Server " + DateTime.Now.ToString("HH:mm:ss.fff"));

        }
    }
}