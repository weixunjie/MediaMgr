﻿using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using System.Timers;
using MediaMgrSystem.DataAccessLayer;
using System.Net;
using System.Net.Sockets;

namespace MediaMgrWS
{

    public enum CommandTypeEnum
    {
        ENCODERREG = 400,
        ENCODERSENDGROUPSINFO = 401,

        ENCODEROPEN = 402,


        ENCODERCLOSE = 403,

        ENCODERQUIT = 406,



    }

    public class ComuResponse
    {
        public string guidId
        {
            get;
            set;
        }

        public string errorCode
        {
            get;
            set;
        }
        public string message
        {
            get;
            set;
        }

    }

    public class EncoderCommandBase
    {
        public string guidId
        {
            get;
            set;
        }

        public CommandTypeEnum commandType
        {
            get;
            set;
        }
        public string clientIdentify
        {
            get;
            set;
        }

    }


    public class EncoderSendGroupsInfoCommand : EncoderCommandBase
    {
        public List<GroupInfo> groups
        {
            get;
            set;
        }

    }


    public class EncoderOperationCommand : EncoderCommandBase
    {
        public List<string> groupIds
        {
            get;
            set;
        }


    }

    public class GroupInfo
    {
        public string groupId { get; set; }
        public string groupName { get; set; }

    }



    public class CommandResponse
    {
        public string guidId
        {
            get;
            set;
        }

        public string errorCode
        {
            get;
            set;
        }
        public string message
        {
            get;
            set;
        }

    }

    public class SocketClients
    {
        public string ClientIdentify { get; set; }
        public Socket SokcetInstance { get; set; }

    }

    public partial class MediaMgrService : ServiceBase
    {

        private HubConnection hubConnection;


        private bool _isConnected = false;


        private const int ADVANCED_START_SECS = 5;
        private const int ADVANCED_STOP_SECS = 1;
        private object lockObject = new object();

        private object lockFlag = new object();

        private object lockQueuItems = new object();


        private object lockClients = new object();

        private DbUtils dbUitls = null;

        private Timer aTimerCheckStartSchedule;

        private Timer aTimerCheckStopSchedule;

        private IHubProxy hubProxy;

        private List<SocketClients> allEncoderClient = new List<SocketClients>();



        public MediaMgrService()
        {

            InitializeComponent();
        }



        void aTimerForStart_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                if (_isConnected)
                {
                    CheckAndStartTask();
                }
            }
        }

        void aTimerForStop_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                if (_isConnected)
                {
                    CheckAndStopTask();
                }
            }
        }


        //   {"guidId":"2847f884-a55b-4375-aca4-a7f2f2df08b9","commandType":"400","clientType":"ENCODER","clientIdentify": "192.168.1.1"}


        //     {"guidId":"2847f884-a55b-4375-aca4-a7f2f2df08b9","commandType":"400","clientType":"ENCODER","clientIdentify": "192.168.1.1"}

        private void CheckAndStartTask()
        {
            string sqlStr = "SELECT * FROM DBO.SCHEDULETASKINFO WHERE  " +
                          " CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)>CONVERT(DATETIME,'{0}') " +
                          " AND (SCHEDULETASKSPECIALDAYS LIKE '%{1}%' OR SCHEDULETASKWEEKS LIKE '%{2}%' ) AND " +
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO) AND ISRUNNING<>1 " +
                           " AND (LASTRUNDATE IS NULL OR LASTRUNDATE='' OR ( DATEDIFF(S, LASTRUNDATE,GETDATE())>" + ADVANCED_START_SECS.ToString() + "))";

            CheckTask(sqlStr, true);
        }


        private void CheckTask(string sqlStr, bool isCheckStart)
        {
            try
            {
                string strWeek = DateTime.Now.DayOfWeek.ToString();
                string weekIndex = string.Empty;
                switch (strWeek)
                {
                    case "Monday":
                        weekIndex = "1";
                        break;
                    case "Tuesday":
                        weekIndex = "2";
                        break;
                    case "Wednesday":
                        weekIndex = "3";
                        break;
                    case "Thursday":
                        weekIndex = "4";
                        break;
                    case "Friday":
                        weekIndex = "5";
                        break;
                    case "Saturday":
                        weekIndex = "6";
                        break;
                    case "Sunday":
                        weekIndex = "7";
                        break;

                }

                string dtMinsSec = DateTime.Now.ToString("1900-01-01 HH:mm:ss");


                sqlStr = String.Format(sqlStr, dtMinsSec, DateTime.Now.ToString("yyyy-MM-dd"), weekIndex);

                DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string strTimeToCheck = isCheckStart ? dt.Rows[i]["ScheduleTaskStartTime"].ToString() : dt.Rows[i]["ScheduleTaskEndTime"].ToString();

                        DateTime dtRunTime;
                        string dtNow = DateTime.Now.ToString("HH:mm:ss");
                        if (DateTime.TryParse(strTimeToCheck, out dtRunTime))
                        {

                            string strScheduleId = dt.Rows[i]["ScheduleId"].ToString();
                            string strScheduleTaskId = dt.Rows[i]["ScheduleTaskId"].ToString();

                            string strIsRepeat = dt.Rows[i]["IsRepeat"].ToString();

                            string strScheduleTaskPriority = dt.Rows[i]["ScheduleTaskPriority"].ToString();

                            TimeSpan tsOffset = dtRunTime.Subtract(DateTime.Parse(dtNow));

                            int andvanceSec = isCheckStart ? ADVANCED_START_SECS : ADVANCED_STOP_SECS;


                            if (tsOffset.TotalSeconds <= andvanceSec && tsOffset.TotalMilliseconds > 0)
                            {
                                string sqlStrGetChannelId = "SELECT CHANNELID,CHANNELNAME FROM DBO.CHANNELINFO WHERE SCHEDULEID='" + strScheduleId + "'";

                                List<string> strChannelIds = new List<string>();
                                List<string> strChannelNames = new List<string>();
                                DataTable dtChannel = dbUitls.ExecuteDataTable(sqlStrGetChannelId);

                                if (dtChannel != null && dtChannel.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dtChannel.Rows.Count; k++)
                                    {
                                        strChannelIds.Add(dtChannel.Rows[k]["CHANNELID"].ToString());
                                        strChannelNames.Add(dtChannel.Rows[k]["CHANNELNAME"].ToString());
                                    }
                                }


                                for (int m = 0; m < strChannelIds.Count; m++)
                                {
                                    string cid = strChannelIds[m];
                                    string cName = strChannelNames[m];
                                    if (!string.IsNullOrEmpty(cid))
                                    {

                                        if (!isCheckStart)
                                        {
                                            string[] strPids = new string[0];

                                            hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "2", strScheduleTaskId, strTimeToCheck, strIsRepeat, strScheduleTaskPriority);

                                            UpdateRunningStatus(false, strScheduleTaskId, true);
                                            System.Diagnostics.Debug.WriteLine("Sending Stop Schedule At " + DateTime.Now.ToString("HH:mm:ss") + "Channel Id:" + cid + " Guid ID" + strScheduleTaskId);

                                        }

                                        else if (isCheckStart)
                                        {
                                            string programeIds = dt.Rows[i]["ScheduleTaskProgarmId"].ToString();

                                            string[] strPids = null;

                                            if (!string.IsNullOrEmpty(programeIds))
                                            {
                                                strPids = programeIds.Split(',');
                                            }

                                            if (!string.IsNullOrEmpty(cid) && strPids != null && strPids.Length > 0)
                                            {

                                                hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "1", strScheduleTaskId, strTimeToCheck, strIsRepeat, strScheduleTaskPriority);

                                                UpdateRunningStatus(true, strScheduleTaskId, true);
                                                System.Diagnostics.Debug.WriteLine("Sending Start Schedule At " + DateTime.Now.ToString("HH:mm:ss") + "Channel Id:" + cid + " Guid ID" + strScheduleTaskId);


                                            }


                                        }

                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("出错拉 " + ex.Message);
            }
        }





        private void CheckAndStopTask()
        {
            string sqlStr = " SELECT * FROM DBO.SCHEDULETASKINFO WHERE  " +
                          " CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)>CONVERT(DATETIME,'{0}') " +
                          " AND (SCHEDULETASKSPECIALDAYS LIKE '%{1}%' OR SCHEDULETASKWEEKS LIKE '%{2}%' ) AND " +
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO) AND ISRUNNING=1 " +
            " AND (LASTSTOPDATE IS NULL OR LASTSTOPDATE='' OR ( DATEDIFF(S, LASTSTOPDATE,GETDATE())>" + ADVANCED_STOP_SECS.ToString() + "))";

            CheckTask(sqlStr, false);
        }

        protected override void OnStart(string[] args)
        {

            dbUitls = new DbUtils(System.Configuration.ConfigurationSettings.AppSettings["ConnStr"].ToString());
            DoConnection();

        }

        SocketPermission permission;
        Socket sListener;
        IPEndPoint ipEndPoint;
        Socket handler;


        private void OpenSocketServer()
        {
            try
            {
                // Creates one SocketPermission object for access restrictions
                permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections 
                TransportType.Tcp,        // Defines transport types 
                "",                       // The IP addresses of local host 
                SocketPermission.AllPorts // Specifies all ports 
                );

                // Listening Socket object 
                sListener = null;

                // Ensures the code to have permission to access a Socket 
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance 
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost 
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint 
                ipEndPoint = new IPEndPoint(ipAddr, 4510);

                // Create one Socket object to listen the incoming connection 
                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint 
                sListener.Bind(ipEndPoint);


                sListener.Listen(10);

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);



            }
            catch (Exception ex) { }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication 
            Socket handler = null;
            try
            {
                // Receiving byte array 
                byte[] buffer = new byte[1024];
                // Get Listening Socket object 
                listener = (Socket)ar.AsyncState;
                // Create a new socket 
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm 
                handler.NoDelay = false;

                // Creates one object array for passing data 
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data 
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    obj            // Specifies infomation for receive operation 
                    );

                // Begins an asynchronous operation to accept an attempt 
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception exc) { }
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information 
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array 
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication. 
                handler = (Socket)obj[1];

                // Received message 
                string content = string.Empty;


                // The number of bytes received. 
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);

                    if (!string.IsNullOrWhiteSpace(content))
                    {

                        EncoderOperationCommand epc = Newtonsoft.Json.JsonConvert.DeserializeObject<EncoderOperationCommand>(content);

                        if (epc != null && epc.groupIds != null && epc.groupIds.Count > 0)
                        {

                        }
                        else
                        {
                            EncoderCommandBase ecb = Newtonsoft.Json.JsonConvert.DeserializeObject<EncoderCommandBase>(content);
                            if (ecb != null && ecb.commandType == CommandTypeEnum.ENCODERREG)
                            {

                                lock (lockClients)
                                {
                                    SocketClients objToRemoved = null;
                                    foreach (var c in allEncoderClient)
                                    {
                                        if (c.ClientIdentify == ecb.clientIdentify)
                                        {
                                            objToRemoved = c;
                                            break;
                                        }
                                    }

                                    if (objToRemoved != null)
                                    {
                                        allEncoderClient.Remove(objToRemoved);
                                    }

                                    allEncoderClient.Add(new SocketClients() { ClientIdentify = ecb.clientIdentify, SokcetInstance = handler });
                                }

                                EncoderSendGroupsInfoCommand cmd = new EncoderSendGroupsInfoCommand();
                                cmd.commandType = CommandTypeEnum.ENCODERSENDGROUPSINFO;
                                cmd.guidId = Guid.NewGuid().ToString();
                                cmd.groups = new List<GroupInfo>();
                                cmd.groups.Add(new GroupInfo { groupId = "1", groupName = "f" });
                                cmd.groups.Add(new GroupInfo { groupId = "3", groupName = "bf" });

                                foreach (var c in allEncoderClient)
                                {

                                    if (c.ClientIdentify != ecb.clientIdentify)
                                        SendSocketDataToClient(content, c.SokcetInstance);
                                }

                                // SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(cmd));

                            }
                            else if (ecb != null && ecb.commandType == CommandTypeEnum.ENCODERQUIT)
                            {
                                lock (lockClients)
                                {
                                    SocketClients objToRemoved = null;
                                    foreach (var c in allEncoderClient)
                                    {
                                        if (c.ClientIdentify == ecb.clientIdentify)
                                        {
                                            objToRemoved = c;
                                            break;
                                        }
                                    }

                                    if (objToRemoved != null)
                                    {
                                        allEncoderClient.Remove(objToRemoved);
                                    }

                                }
                            }

                        }
                    }


                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {

                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }



                }
            }
            catch (Exception exc) { }
        }


        private void SendSocketDataToClient(string str, Socket socketToSend)
        {
            try
            {

                // Prepare the reply message 
                byte[] byteData =
                    Encoding.Unicode.GetBytes(str);

                // Sends data asynchronously to a connected Socket 
                socketToSend.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), socketToSend);


            }
            catch (Exception exc) { }
        }

        public void SendCallback(IAsyncResult ar)
        {
            try
            {
                // A Socket which has sent the data to remote host 
                Socket handler = (Socket)ar.AsyncState;

                // The number of bytes sent to the Socket 
                int bytesSend = handler.EndSend(ar);

            }
            catch (Exception exc) { }
        }

        private void CloseSocketServer()
        {
            try
            {
                if (sListener!=null)
                {
                    sListener.Shutdown(SocketShutdown.Both);
                    sListener.Close();
                }


            }
            catch (Exception exc) { }
        }


        private void DoConnection()
        {
            string strSvrUrl = System.Configuration.ConfigurationSettings.AppSettings["ServerUrl"].ToString();
            hubConnection = new HubConnection(strSvrUrl, "clientIdentify=NA&clientType=WINDOWSSERVICE");


            hubProxy = hubConnection.CreateHubProxy("MediaMgrHub");

            hubConnection.Start();

            hubProxy.On<string>("sendMessageToWindowService", (data) =>
            {
                lock (lockQueuItems)
                {
                    ComuResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ComuResponse>(data);


                    if (resp != null && !string.IsNullOrWhiteSpace(resp.guidId))
                    {

                        System.Diagnostics.Debug.WriteLine("Received Message From BS Server at " + DateTime.Now.ToString("HH:mm:ss") + " Error:" + resp.errorCode + resp.message + " Guid ID" + resp.guidId);

                        //Start video, no need remove queue
                        if (resp.errorCode != "181")
                        {
                            UpdateRunningStatus(false, resp.guidId);
                        }
                    }

                }


            });

            hubConnection.StateChanged -= hubConnection_StateChanged;
            hubConnection.StateChanged += hubConnection_StateChanged;

        }

        private void UpdateRunningStatus(bool isRunning, string schduelTaskId, bool isUpdateTime = false)
        {
            string sqlUpdate = " UPDATE DBO.SCHEDULETASKINFO SET ISRUNNING={0}{1} WHERE  SCHEDULETASKID={2} ";

            string updateStr = string.Empty;
            if (isRunning)
            {
                updateStr = ",LASTRUNDATE=GETDATE()";

            }
            else
            {
                updateStr = ",LASTSTOPDATE=GETDATE()";
            }


            sqlUpdate = string.Format(sqlUpdate, isRunning ? 1 : 0, isUpdateTime ? updateStr : "", schduelTaskId);

            dbUitls.ExecuteNonQuery(sqlUpdate);
        }

        void hubConnection_StateChanged(StateChange obj)
        {

            if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {


                lock (lockFlag)
                {
                    _isConnected = true;
                }

                aTimerCheckStartSchedule = new Timer();
                aTimerCheckStartSchedule.Elapsed += aTimerForStart_Elapsed;
                aTimerCheckStartSchedule.Interval = 1 * 800;
                aTimerCheckStartSchedule.Enabled = true;


                aTimerCheckStopSchedule = new Timer();
                aTimerCheckStopSchedule.Elapsed += aTimerForStop_Elapsed;
                aTimerCheckStopSchedule.Interval = 1 * 800;
                aTimerCheckStopSchedule.Enabled = true;


                OpenSocketServer();



            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                CloseSocketServer();

                if (aTimerCheckStartSchedule != null)
                {
                    aTimerCheckStartSchedule.Stop();
                }

                if (aTimerCheckStopSchedule != null)
                {
                    aTimerCheckStopSchedule.Stop();
                }

                lock (lockFlag)
                {
                    _isConnected = false;
                }


                System.Threading.Thread.Sleep(10000);


                if (!_isConnected)
                {
                    DoConnection();
                }


            }
        }

        protected override void OnStop()
        {
            if (aTimerCheckStartSchedule != null)
            {
                aTimerCheckStartSchedule.Stop();
            }

            if (aTimerCheckStopSchedule != null)
            {
                aTimerCheckStopSchedule.Stop();
            }
            if (hubConnection != null)
            {
                hubConnection.Stop();
            }

            CloseSocketServer();


        }


    }
}
