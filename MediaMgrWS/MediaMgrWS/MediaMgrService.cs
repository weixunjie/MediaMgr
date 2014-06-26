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

namespace MediaMgrWS
{

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

    public class TaskInfo
    {
        public string TaskId { get; set; }

        public string RuningTime { get; set; }

        public string GuidId { get; set; }

        public string ChannelId { get; set; }

    }
    public partial class MediaMgrService : ServiceBase
    {

        private HubConnection hubConnection;
        List<TaskInfo> runningTask = new List<TaskInfo>();

        private bool _isConnected = false;

        private TaskInfo _lastRunStartTime = null;

        private TaskInfo _lastRunEndTime = null;

        private object lockObject = new object();

        private object lockFlag = new object();

        private object lockQueuItems = new object();

        private DbUtils dbUitls = null;

        private Timer aTimerCheckStartSchedule;

        private Timer aTimerCheckStopSchedule;

        private IHubProxy hubProxy;
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

        private void CheckAndStartTask()
        {
            string sqlStr = "SELECT * FROM DBO.SCHEDULETASKINFO WHERE  " +
                          " CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)>CONVERT(DATETIME,'{0}') " +
                          " AND (SCHEDULETASKSPECIALDAYS LIKE '%{1}%' OR SCHEDULETASKWEEKS LIKE '%{2}%' ) AND " +
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO) ";

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

                string dtMins = DateTime.Now.ToString("1900-01-01 HH:mm");


                sqlStr = String.Format(sqlStr, dtMins, DateTime.Now.ToString("yyyy-MM-dd"), weekIndex);

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

                            TimeSpan tsOffset = dtRunTime.Subtract(DateTime.Parse(dtNow));

                            //Schedule to play: send 5s in advance
                            //Scehdue to stop: send 1s in advance
                            int andvanceSec = 2;

                            if (isCheckStart)
                            {
                                andvanceSec = 7;
                            }


                            if (tsOffset.TotalSeconds <= andvanceSec && tsOffset.TotalMilliseconds > 0)
                            {
                                if (isCheckStart)
                                {
                                    if (_lastRunStartTime != null &&
                                        _lastRunStartTime.RuningTime == strTimeToCheck &&
                                        _lastRunStartTime.TaskId == strScheduleTaskId
                                        )
                                    {
                                        ///Don't send repeat command
                                        return;

                                    }

                                    _lastRunStartTime = _lastRunStartTime ?? new TaskInfo();

                                    _lastRunStartTime.RuningTime = strTimeToCheck;

                                    _lastRunStartTime.TaskId = strScheduleTaskId;

                                }
                                else
                                {

                                    if (_lastRunEndTime != null &&
                                      _lastRunEndTime.RuningTime == strTimeToCheck &&
                                      _lastRunEndTime.TaskId == strScheduleTaskId
                                      )
                                    {
                                        ///Don't send repeat command
                                        return;
                                    }

                                    _lastRunEndTime = _lastRunStartTime ?? new TaskInfo();
                                    _lastRunEndTime.RuningTime = strTimeToCheck;

                                    _lastRunEndTime.TaskId = strScheduleTaskId;
                                }



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
                                        bool foundRunningTask = false;
                                        foreach (var task in runningTask)
                                        {
                                            if (task.TaskId == strScheduleTaskId && task.ChannelId == cid)
                                            {
                                                foundRunningTask = true;
                                                break;
                                            }
                                        }

                                        if (foundRunningTask && !isCheckStart)
                                        {

                                            TaskInfo taskToRemove = FindAndRemoveTask(strScheduleTaskId, cid);

                                            string[] strPids = new string[0];
                                            //Stop
                                            hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "2", taskToRemove != null ? taskToRemove.GuidId : string.Empty, strTimeToCheck);

                                            System.Diagnostics.Debug.WriteLine("Sending Stop Schedule At " + DateTime.Now.ToString("HH:mm:ss") + "Channel Id:" + cid + " Guid ID" + taskToRemove.GuidId);

                                        }

                                        else if (isCheckStart)
                                        {

                                            TaskInfo taskToRemove = FindAndRemoveTask(strScheduleTaskId, cid);

                                            string programeIds = dt.Rows[i]["ScheduleTaskProgarmId"].ToString();

                                            string[] strPids = null;

                                            if (!string.IsNullOrEmpty(programeIds))
                                            {
                                                strPids = programeIds.Split(',');
                                            }

                                            if (!string.IsNullOrEmpty(cid) && strPids != null && strPids.Length > 0)
                                            {
                                                string strGuid = Guid.NewGuid().ToString();


                                                lock (lockQueuItems)
                                                {

                                                    runningTask.Add(new TaskInfo() { GuidId = strGuid, TaskId = strScheduleTaskId, RuningTime = strTimeToCheck, ChannelId = cid });

                                                }
                                                //Start 

                                                System.Diagnostics.Debug.WriteLine("Sending Start Schedule At " + DateTime.Now.ToString("HH:mm:ss") + "Channel Id:" + cid + " Guid ID" + strGuid);
                                                hubProxy.Invoke("sendScheduleTaskControl", cid, cName, strPids, "1", strGuid, strTimeToCheck);
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

        private TaskInfo FindAndRemoveTask(string strScheduleTaskId, string cid)
        {

            lock (lockQueuItems)
            {
                TaskInfo taskToRemove = null;

                if (runningTask != null && runningTask.Count > 0)
                {
                    foreach (var task in runningTask)
                    {
                        if (task.TaskId == strScheduleTaskId && task.ChannelId == cid)
                        {
                            taskToRemove = task;
                        }
                    }

                }

                if (taskToRemove != null)
                {
                    runningTask.Remove(taskToRemove);
                }

                return taskToRemove;
            }

            return null;


        }




        private void CheckAndStopTask()
        {
            string sqlStr = " SELECT * FROM DBO.SCHEDULETASKINFO WHERE  " +
                          " CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)>CONVERT(DATETIME,'{0}') " +
                          " AND (SCHEDULETASKSPECIALDAYS LIKE '%{1}%' OR SCHEDULETASKWEEKS LIKE '%{2}%' ) AND " +
                          "SCHEDULEID IN(SELECT DISTINCT SCHEDULEID FROM DBO.CHANNELINFO) ";


            CheckTask(sqlStr, false);
        }

        protected override void OnStart(string[] args)
        {
            runningTask = new List<TaskInfo>();
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
                            TaskInfo taskToRemove = null;

                            if (runningTask != null && runningTask.Count > 0)
                            {
                                foreach (var task in runningTask)
                                {
                                    if (task.GuidId == resp.guidId)
                                    {
                                        taskToRemove = task;
                                    }
                                }

                            }

                            if (taskToRemove != null)
                            {
                                runningTask.Remove(taskToRemove);
                            }
                        }
                    }

                }


            });

            hubConnection.StateChanged -= hubConnection_StateChanged;
            hubConnection.StateChanged += hubConnection_StateChanged;

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



            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
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
                System.Threading.Thread.Sleep(4000);

                if (!_isConnected)
                {
                    DoConnection();
                }
            }
        }

        protected override void OnStop()
        {
            aTimerCheckStartSchedule.Stop();
            aTimerCheckStopSchedule.Stop();
            if (hubConnection != null)
            {
                hubConnection.Stop();
            }


        }


    }
}
