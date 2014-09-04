using Microsoft.AspNet.SignalR.Client;
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



    public class RunningTask
    {
        public string ChannelId { get; set; }

        public string ScheduleTaskId { get; set; }
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

        private object lockRunningtems = new object();

        private DbUtils dbUitls = null;

        private Timer aTimerCheckStartSchedule;

        private Timer aTimerCheckStopSchedule;

        private IHubProxy hubProxy;

        private List<RunningTask> TasksRunning = new List<RunningTask>();

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
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO) " +
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
                                            if (!CheckIfRunning(strScheduleTaskId, cid))
                                            {
                                                continue;
                                            }

                                            string[] strPids = new string[0];

                                            hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "2", strScheduleTaskId, strTimeToCheck, strIsRepeat, strScheduleTaskPriority);

                                            UpdateRunningStatus(false, strScheduleTaskId, cid, true);
                                            System.Diagnostics.Debug.WriteLine("Sending Stop Schedule At " + DateTime.Now.ToString("HH:mm:ss") + "Channel Id:" + cid + " Guid ID" + strScheduleTaskId);

                                        }

                                        else if (isCheckStart)
                                        {

                                            if (CheckIfRunning(strScheduleTaskId, cid))
                                            {
                                                continue;
                                            }

                                            string programeIds = dt.Rows[i]["ScheduleTaskProgarmId"].ToString();

                                            string[] strPids = null;

                                            if (!string.IsNullOrEmpty(programeIds))
                                            {
                                                strPids = programeIds.Split(',');
                                            }

                                            if (!string.IsNullOrEmpty(cid) && strPids != null && strPids.Length > 0)
                                            {

                                                hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "1", strScheduleTaskId, strTimeToCheck, strIsRepeat, strScheduleTaskPriority);

                                                UpdateRunningStatus(true, strScheduleTaskId, cid, true);
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
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO)  " +
            " AND (LASTSTOPDATE IS NULL OR LASTSTOPDATE='' OR ( DATEDIFF(S, LASTSTOPDATE,GETDATE())>" + ADVANCED_STOP_SECS.ToString() + "))";

            CheckTask(sqlStr, false);
        }

        protected override void OnStart(string[] args)
        {

            TasksRunning = new List<RunningTask>();
            dbUitls = new DbUtils(System.Configuration.ConfigurationSettings.AppSettings["ConnStr"].ToString());
            DoConnection();



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

                            string[] strs = resp.guidId.Split(',');
                            UpdateRunningStatus(false, strs[0], strs[1]);
                        }
                    }

                }

                lock (EncoderSocketLogic.lockEncoderCommand)
                {
                    EncoderCommandBase encoderRequestCmd = Newtonsoft.Json.JsonConvert.DeserializeObject<EncoderCommandBase>(data);


                    if (encoderRequestCmd != null && !string.IsNullOrWhiteSpace(encoderRequestCmd.guidId))
                    {

                        EncoderSocketLogic.allQueueCommandToEncoder.Add(new ToEncoderCommand() { ClientIdentify = encoderRequestCmd.clientIdentify, GuidId = encoderRequestCmd.guidId });

                        SocketClients sc = EncoderSocketLogic.GetConnectedEcoderSocket(encoderRequestCmd.clientIdentify);

                        if (sc != null)
                        {
                            EncoderSocketLogic.SendSocketDataToClient(Newtonsoft.Json.JsonConvert.SerializeObject(sc), sc.SokcetInstance);
                        }

                    }

                }


            });

            hubConnection.StateChanged -= hubConnection_StateChanged;
            hubConnection.StateChanged += hubConnection_StateChanged;

        }

        private bool CheckIfRunning(string schduelTaskId, string channelId)
        {

            lock (lockRunningtems)
            {
                foreach (var task in TasksRunning)
                {
                    if (task.ChannelId == channelId && task.ScheduleTaskId == schduelTaskId)
                    {
                        return true;
                    }
                }

            }

            return false;



        }
        private void UpdateRunningStatus(bool isRunning, string schduelTaskId, string channelId, bool isUpdateTime = false)
        {


            string updateStr = string.Empty;

            if (isRunning)
            {
                updateStr = "UPDATE DBO.SCHEDULETASKINFO  set LASTRUNDATE=GETDATE() WHERE  SCHEDULETASKID={0} ";

            }
            else
            {
                updateStr = "UPDATE DBO.SCHEDULETASKINFO  set LASTSTOPDATE=GETDATE() WHERE  SCHEDULETASKID={0} ";
            }

            if (isUpdateTime)
            {
                updateStr = string.Format(updateStr, schduelTaskId);

                dbUitls.ExecuteNonQuery(updateStr);
            }

            lock (lockRunningtems)
            {


                RunningTask itemToRemove = null;
                foreach (var task in TasksRunning)
                {
                    if (task.ChannelId == channelId && task.ScheduleTaskId == schduelTaskId)
                    {
                        itemToRemove = task;
                        break;
                    }
                }



                if (isRunning)
                {

                    TasksRunning.Add(new RunningTask { ChannelId = channelId, ScheduleTaskId = schduelTaskId });


                }
                else
                {
                    if (itemToRemove != null)
                    {
                        TasksRunning.Remove(itemToRemove);
                    }
                }
            }

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




                EncoderSocketLogic.EncoderBLLInstance = new EncoderBLL(dbUitls);

                EncoderSocketLogic.hubProxy = hubProxy;

                EncoderSocketLogic.OpenSocketServer();



            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                EncoderSocketLogic.CloseSocketServer();

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

            EncoderSocketLogic.CloseSocketServer();


        }


    }
}
