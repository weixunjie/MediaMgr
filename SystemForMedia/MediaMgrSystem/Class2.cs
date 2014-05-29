using MediaMgrSystem.DataModels;
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
        public static void SetCommand(int cd,dynamic cs)
        {

            //DeviceRequest dr = new DeviceRequest();

            //dr.streamSrcs = new List<String>();

            //dr.streamSrcs.Add("test.mp4");
            //dr.streamSrcs.Add("demo.mp3");

            //dr.broadcastFlag = "1";
            //dr.deviceIP = ipAddress;
            //dr.guidId = Guid.NewGuid().ToString();// DateTime.Now.ToString("HH:mm:ss.fff");
            //dr.commandType = commandType;//"TURNONOFF";

            //dr.arg = new DeviceControlCommand();
            //dr.arg.streamSrc = "test.mp4";
            //dr.arg.udpBroadcastAddress="udp://udp://229.0.0.1:5000";


            //ScheduleRequest sr = new ScheduleRequest();
            //sr.commandType = "4";
            //sr.guidId = Guid.NewGuid().ToString();

            //sr.streamSrcs = new List<ScheduleTask>();

            //ScheduleTask st = new ScheduleTask();
            //st.beginTime = "08:00";
            //st.endTime = "08:10";
            //st.buffer = "2";
            //st.repeatWeekDays = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };
            //st.streamSrcs = new List<string>() { "test.mp4", "demo.mp4" };
            //st.specialDays = new List<string>() { "2014-05-12", "2014-05-12" };
            //st.isRepeat = "1";


            //ScheduleTask st1 = new ScheduleTask();
            //st1.beginTime = "09:00";
            //st1.endTime = "09:10";
            //st1.buffer = "2";
            //st1.repeatWeekDays = new List<string>() { "1", "2", "3", "4", "5", "6" };
            //st1.streamSrcs = new List<string>() { "play.mp4", "advedio.mp4" };
            //st1.specialDays = new List<string>() { "2014-06-1", "2014-06-1" };
            //st1.isRepeat = "0";

            //sr.streamSrcs.Add(st);
            //sr.streamSrcs.Add(st1);


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
            cs.All.sendAllMessge(Newtonsoft.Json.JsonConvert.SerializeObject(vo));

            System.Diagnostics.Debug.WriteLine("End Send to Client " + DateTime.Now.ToString("HH:mm:ss.fff"));

            System.Diagnostics.Debug.WriteLine("Begin Send to Server " + DateTime.Now.ToString("HH:mm:ss.fff"));
            String str= Newtonsoft.Json.JsonConvert.SerializeObject(voc);
            System.Diagnostics.Debug.WriteLine("Send to Server Done Json Serizalation" + DateTime.Now.ToString("HH:mm:ss.fff"));
     
            System.Net.HttpWebRequest httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(System.Configuration.ConfigurationManager.AppSettings["VideoUrl"].ToString() + "?PushTask=" + str);


            httpWebRequest.KeepAlive = true;

            System.Diagnostics.Debug.WriteLine("Send to Server Done Init web request" + DateTime.Now.ToString("HH:mm:ss.fff"));
     

            httpWebRequest.ContentType = "application/json; charset=utf-8";

            var response = httpWebRequest.GetResponse();

            System.Diagnostics.Debug.WriteLine("Send to Server Done Get Response" + DateTime.Now.ToString("HH:mm:ss.fff"));

            Stream streamResponse = response.GetResponseStream();

            System.Diagnostics.Debug.WriteLine("Send to Server Done Get Stream" + DateTime.Now.ToString("HH:mm:ss.fff"));

            StreamReader streamRead = new StreamReader(streamResponse);

            String responseString = streamRead.ReadToEnd();
            System.Diagnostics.Debug.WriteLine("Send to Server Done Convert Stream 2 String" + DateTime.Now.ToString("HH:mm:ss.fff"));
            System.Diagnostics.Debug.WriteLine("End Send to Server " + DateTime.Now.ToString("HH:mm:ss.fff"));
            //    ComuResponseBase cb = Newtonsoft.Json.JsonConvert.DeserializeObject<ComuResponseBase>(responseString);
            // hr = new HttpRequest();


            //if (responseString == "0" && cd == 1)
            //{

            //    Clients.All.sendAllMessge(Newtonsoft.Json.JsonConvert.SerializeObject(vo));
            //}
        }
    }
}