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

        private object lockObjet = new object();

        private DbUtils dbUitls = null;
        private Timer aTimer;
        private IHubProxy hubProxy;
        public MediaMgrService()
        {

            InitializeComponent();
        }

        void aTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObjet)
            {

                string dtMins = DateTime.Now.AddMinutes(2).ToString("1900-01-01 HH:mm");

                string strWeek = DateTime.Today.DayOfWeek.ToString();

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

                CheckAndStartTask(dtMins, weekIndex);
                CheckAndStopTask(dtMins, weekIndex);

            }
        }

        private void CheckAndStartTask(string dtMins, string weekIndex)
        {
            string sqlStr = " select * from dbo.ScheduleTaskInfo where  " +
                          " CONVERT(datetime,'1900-01-01 '+ ScheduleTaskStartTime)>CONVERT(datetime,'{0}') " +
                          " and (ScheduleTaskspecialDays like '%{1}' or ScheduleTaskWeeks like '%{2}' ) and " +
                          "ScheduleTaskId in(select distinct ScheduleId from dbo.ChannelInfo) ";

            sqlStr = String.Format(sqlStr, dtMins, DateTime.Now.ToString("yyyy-MM-dd"), weekIndex);

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strRunningTime = dt.Rows[i]["ScheduleTaskStartTime"].ToString();

                    DateTime dtRunTime;
                    string dtNow = DateTime.Now.ToString("HH:mm:ss");
                    if (DateTime.TryParse(strRunningTime, out dtRunTime))
                    {

                        string strScheduleId = dt.Rows[i]["ScheduleId"].ToString();
                        string strScheduleTaskId = dt.Rows[i]["ScheduleTaskId"].ToString();

                        string programeIds = dt.Rows[i]["ScheduleTaskProgarmId"].ToString();

                        TimeSpan tsOffset = DateTime.Parse(dtNow).Subtract(dtRunTime).Duration();
                        if (tsOffset.TotalSeconds <= 5)
                        {
                            bool foundRunningTask = false;
                            foreach (var task in runningTask)
                            {
                                if (task.TaskId == strScheduleTaskId)
                                {
                                    foundRunningTask = true;
                                    break;
                                }
                            }

                            if (!foundRunningTask)
                            {

                                string sqlStrGetChannelId = " select ChannelId from dbo.ChannelInfo where ScheduleId=' " + strScheduleId + "'";


                                string cId = string.Empty;
                                DataTable dtCId = dbUitls.ExecuteDataTable(sqlStrGetChannelId);

                                if (dtCId != null && dt.Rows != null && dt.Rows.Count > 0)
                                {
                                    cId = dtCId.Rows[0][0].ToString();
                                }

                                string[] strPids = null;

                                if (!string.IsNullOrEmpty(programeIds))
                                {
                                    strPids = programeIds.Split(',');
                                }

                                if (!string.IsNullOrEmpty(cId) && strPids != null && strPids.Length > 0)
                                {

                                    runningTask.Add(new TaskInfo() { TaskId = strScheduleTaskId, RuningTime = strRunningTime, ChannelId = cId });

                                    hubProxy.Invoke("sendScheduleTaskControl", cId, strPids, 1);

                                }

                            }
                        }
                    }
                }

            }
        }



        private void CheckAndStopTask(string dtMins, string weekIndex)
        {
            string sqlStr = " select * from dbo.ScheduleTaskInfo where  " +
                          " CONVERT(datetime,'1900-01-01 '+ ScheduleTaskEndTime)>CONVERT(datetime,'{0}') " +
                          " and (ScheduleTaskspecialDays like '%{1}' or ScheduleTaskWeeks like '%{2}' ) and " +
                          "ScheduleTaskId in(select distinct ScheduleId from dbo.ChannelInfo) ";

            sqlStr = String.Format(sqlStr, dtMins, DateTime.Now.ToString("yyyy-MM-dd"), weekIndex);

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strRunningTime = dt.Rows[i]["ScheduleTaskStartTime"].ToString();

                    DateTime dtRunTime;
                    string dtNow = DateTime.Now.ToString("HH:mm:ss");
                    if (DateTime.TryParse(strRunningTime, out dtRunTime))
                    {

                        string strScheduleId = dt.Rows[i]["ScheduleId"].ToString();
                        string strScheduleTaskId = dt.Rows[i]["ScheduleTaskId"].ToString();

                        TimeSpan tsOffset = DateTime.Parse(dtNow).Subtract(dtRunTime).Duration();
                        if (tsOffset.TotalSeconds <= 1)
                        {
                            bool foundRunningTask = false;
                            foreach (var task in runningTask)
                            {
                                if (task.TaskId == strScheduleTaskId)
                                {
                                    foundRunningTask = true;
                                    break;
                                }
                            }

                            if (foundRunningTask)
                            {

                                string sqlStrGetChannelId = " select ChannelId from dbo.ChannelInfo where ScheduleId=' " + strScheduleId + "'";


                                string cId = string.Empty;
                                DataTable dtCId = dbUitls.ExecuteDataTable(sqlStrGetChannelId);


                                if (!string.IsNullOrEmpty(cId))
                                {
                                    TaskInfo taskToRemove = null;

                                    if (runningTask != null && runningTask.Count > 0)
                                    {
                                        foreach (var task in runningTask)
                                        {
                                            if (task.TaskId == strScheduleId)
                                            {
                                                taskToRemove = task;
                                            }
                                        }

                                    }

                                    if (taskToRemove != null)
                                    {
                                        runningTask.Remove(taskToRemove);
                                    }

                                    hubProxy.Invoke("sendScheduleTaskControl", cId, null, 2);

                                }

                            }
                        }
                    }
                }

            }
        }

        protected override void OnStart(string[] args)
        {
            runningTask = new List<TaskInfo>();
            dbUitls = new DbUtils(System.Configuration.ConfigurationSettings.AppSettings["ConnStr"].ToString());
            DoConnection();
        }

        private void DoConnection()
        {

            hubConnection = new HubConnection("http://localhost/MediaMgrDemo");

            hubProxy = hubConnection.CreateHubProxy("Test");

            hubConnection.Start();


            hubProxy.On<string>("sendMessageToWindowService", (data) =>
            {

                ComuResponse resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ComuResponse>(data);

                if (resp != null && !string.IsNullOrWhiteSpace(resp.guidId))
                {
                    lock (lockObjet)
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




            });

            hubConnection.StateChanged -= hubConnection_StateChanged;
            hubConnection.StateChanged += hubConnection_StateChanged;

        }
        void hubConnection_StateChanged(StateChange obj)
        {

            if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                aTimer = new Timer();
                aTimer.Elapsed += aTimer_Elapsed;
                aTimer.Interval = 1 * 800;
                aTimer.Enabled = true;

            }
            else if (obj.NewState == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
            {
                DoConnection();
            }
        }

        protected override void OnStop()
        {
            aTimer.Stop();
            if (hubConnection != null)
            {
                hubConnection.Stop();
            }
        }


    }
}
