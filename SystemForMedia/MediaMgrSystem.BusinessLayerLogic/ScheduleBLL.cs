using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.BusinessLayerLogic
{
    public class ScheduleBLL
    {

        DbUtils dbUitls = null;


        public ScheduleBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public List<ScheduleInfo> GetAllSchedules()
        {
            String sqlStr = "SELECT * FROM SCHEDULEINFO";

            return GetAllScheduleList(sqlStr);

        }

        public List<ScheduleInfo> GetAllScheduleById(string id)
        {

            String sqlStr = "SELECT * FROM SCHEDULEINFO WHERE SCHEDULEID='" + id + "'";

            return GetAllScheduleList(sqlStr);

        }


        public bool CheckScheduleTaskIsRunning(string schduduleTaskId)
        {

            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE ISRUNNING=1 AND SCHEDULETASKID= " + schduduleTaskId;

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    return true;
                }
            }

            return false;


        }

        public bool CheckScheduleTaskIsByScheduleId(string schduduled)
        {

            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE ISRUNNING=1 AND SCHEDULEID= " + schduduled;

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    return true;
                }
            }

            return false;


        }




        public bool CheckProgrameIsUsing(string pid)
        {
            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE  SCHEDULETASKPROGARMID='" + pid + "'";

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    return true;
                }
            }

            return false;

        }

        public bool CheckScheduleTaskTimeIsOverLap(string schduduleId, string startTimeToCheck, string endTimeToCheck, string exceptTaskId, string sqlCheckWks)
        {



            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE ((CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)>=CONVERT(DATETIME,'{0}') "
            + " AND  CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)<=CONVERT(DATETIME,'{0}')) OR "
            + " (CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)>=CONVERT(DATETIME,'{1}') "
            + " AND  CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)<=CONVERT(DATETIME,'{1}')) OR "
            + " (CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)>= CONVERT(DATETIME,'{0}') "
            + " AND  CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKSTARTTIME)<=CONVERT(DATETIME,'{1}')) OR "
            + " ( CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)>=CONVERT(DATETIME,'{0}') "
            + " AND CONVERT(DATETIME,'1900-01-01 '+ SCHEDULETASKENDTIME)<=CONVERT(DATETIME,'{1}'))) " +
            " AND SCHEDULEID='{2}'" + sqlCheckWks;

            sqlStr = string.Format(sqlStr, "1900-01-01 " + startTimeToCheck, "1900-01-01 " + endTimeToCheck, schduduleId);

            if (!string.IsNullOrEmpty(exceptTaskId))
            {
                sqlStr = sqlStr + " AND SCHEDULETASKID<>'" + exceptTaskId + "'";
            }

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    return true;
                }
            }

            return false;

        }


        public int RemoveScheduleTaskOnly(string id)
        {


            String sqlRemoveTaskStr = "DELETE FROM SCHEDULETASKINFO WHERE SCHEDULEID=" + id;

            return dbUitls.ExecuteNonQuery(sqlRemoveTaskStr);

        }


        public int RemoveSchedule(string id)
        {
            String sqlStr = "DELETE FROM SCHEDULEINFO WHERE SCHEDULEID=" + id;

            dbUitls.ExecuteNonQuery(sqlStr);

            String sqlRemoveTaskStr = "DELETE FROM SCHEDULETASKINFO WHERE SCHEDULEID=" + id;

            return dbUitls.ExecuteNonQuery(sqlRemoveTaskStr);

        }

        public int AddSchedule(ScheduleInfo si)
        {
            String sqlStr = "INSERT INTO SCHEDULEINFO(SCHEDULENAME) values ('{0}')";

            sqlStr = String.Format(sqlStr, si.ScheduleName);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateSchedule(ScheduleInfo si)
        {
            String sqlStr = "UPDATE SCHEDULEINFO SET SCHEDULENAME='{0}' WHERE SCHEDULEID={1}";

            sqlStr = String.Format(sqlStr, si.ScheduleName, si.ScheduleId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<ScheduleInfo> GetAllScheduleList(string sqlStr)
        {

            List<ScheduleInfo> sInfos = new List<ScheduleInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ScheduleInfo si = new ScheduleInfo();
                        si.ScheduleId = dt.Rows[i]["SCHEDULEID"].ToString();
                        si.ScheduleName = dt.Rows[i]["SCHEDULENAME"].ToString();


                        sInfos.Add(si);
                    }

                }
            }

            return sInfos;

        }

        public List<ScheduleTaskInfo> GetAllScheduleTaksByScheduleId(string id)
        {
            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE SCHEDULEID='" + id + "' order by ScheduleTaskStartTime ";
            return GetTasksList(sqlStr);

        }

        public List<ScheduleTaskInfo> GetAllScheduleTask()
        {

            String sqlStr = "SELECT * FROM SCHEDULETASKINFO";

            return GetTasksList(sqlStr);

        }

        public ScheduleTaskInfo GetNextScheduleTask()
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

            string sqlStr = " SELECT TOP 1 * FROM SCHEDULETASKINFO  WHERE SCHEDULETASKSTARTTIME >'{0}' AND (SCHEDULETASKSPECIALDAYS LIKE '%{1}%' OR SCHEDULETASKWEEKS LIKE '%{2}%') AND   SCHEDULEID IN (SELECT DISTINCT  SCHEDULEID FROM CHANNELINFO)    ORDER BY SCHEDULETASKSTARTTIME  ";

            sqlStr = String.Format(sqlStr, DateTime.Now.ToString("HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd"), weekIndex);

            List<ScheduleTaskInfo> result = GetTasksList(sqlStr);

            if (result != null && result.Count > 0)
            {
                return result[0];
            }

            return null;

        }



        public ScheduleTaskInfo GetAllScheduleTaskById(string id)
        {

            String sqlStr = "SELECT * FROM SCHEDULETASKINFO WHERE SCHEDULETASKID='" + id + "'";

            List<ScheduleTaskInfo> result = GetTasksList(sqlStr);
            if (result != null && result.Count > 0)
            {
                return result[0];
            }

            return null;

        }


        public int RemoveSchdeulTask(string id)
        {
            String sqlStr = "DELETE FROM SCHEDULETASKINFO WHERE SCHEDULETASKID=" + id;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddSchdeulTask(ScheduleTaskInfo si)
        {
            String sqlStr = "INSERT INTO SCHEDULETASKINFO(SCHEDULEID,SCHEDULETASKSTARTTIME,SCHEDULETASKENDTIME,SCHEDULETASKPROGARMID,SCHEDULETASKPRIORITY,SCHEDULETASKWEEKS,SCHEDULETASKSPECIALDAYS,SCHEDULETASKNAME,SCHEDULETASKSPECIALDAYSTOWEEKS,ISRUNNING,ISREPEAT,IsForAudio) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',0,{9},{10})";



            sqlStr = String.Format(sqlStr, si.ScheduleId, si.ScheduleTaskStartTime, si.ScheduleTaskEndTime, si.ScheduleTaskProgarmId, si.ScheduleTaskPriority, si.StrWeeks, si.StrDays, si.ScheduleTaskName, si.StrSpecialDaysToWeeks, si.IsRepeat, si.IsForAudio);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateScheduleTask(ScheduleTaskInfo si)
        {
            String sqlStr = "UPDATE SCHEDULETASKINFO SET SCHEDULEID='{0}',SCHEDULETASKSTARTTIME='{1}',SCHEDULETASKENDTIME='{2}',SCHEDULETASKPROGARMID='{3}',SCHEDULETASKPRIORITY='{4}',SCHEDULETASKWEEKS='{5}',SCHEDULETASKSPECIALDAYS='{6}',SCHEDULETASKNAME='{7}',SCHEDULETASKSPECIALDAYSTOWEEKS='{8}',ISREPEAT={9},IsForAudio={10} WHERE SCHEDULETASKID={11}";

            sqlStr = String.Format(sqlStr, si.ScheduleId, si.ScheduleTaskStartTime, si.ScheduleTaskEndTime, si.ScheduleTaskProgarmId, si.ScheduleTaskPriority, si.StrWeeks, si.StrDays, si.ScheduleTaskName, si.StrSpecialDaysToWeeks, si.IsRepeat, si.IsForAudio, si.ScheduleTaskId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<ScheduleTaskInfo> GetTasksList(string sqlStr)
        {
            List<ScheduleTaskInfo> tasks = new List<ScheduleTaskInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ScheduleTaskInfo si = new ScheduleTaskInfo();

                        si.ScheduleId = dt.Rows[i]["SCHEDULEID"].ToString();
                        si.ScheduleTaskId = dt.Rows[i]["SCHEDULETASKID"].ToString();
                        si.ScheduleTaskStartTime = dt.Rows[i]["SCHEDULETASKSTARTTIME"].ToString();
                        si.ScheduleTaskEndTime = dt.Rows[i]["SCHEDULETASKENDTIME"].ToString();
                        si.ScheduleTaskProgarmId = dt.Rows[i]["SCHEDULETASKPROGARMID"].ToString();

                        si.ScheduleTaskPriority = dt.Rows[i]["SCHEDULETASKPRIORITY"].ToString();

                        si.ScheduleTaskName = dt.Rows[i]["SCHEDULETASKNAME"].ToString();

                        si.IsRepeat = dt.Rows[i]["IsRepeat"].ToString();

                        string strIsForAudio = dt.Rows[i]["IsForAudio"].ToString();

                        si.IsForAudio = 0;
                        
                        int tmpIntForAuido;
                        if (int.TryParse(strIsForAudio, out tmpIntForAuido))
                        {
                            si.IsForAudio = tmpIntForAuido;
                        }



                        string strWks = dt.Rows[i]["SCHEDULETASKWEEKS"].ToString();

                        si.StrWeeks = strWks;
                        si.ScheduleTaskWeeks = new List<string>();
                        if (!string.IsNullOrWhiteSpace(strWks))
                        {
                            string[] tmpSplit = strWks.Split(',');

                            if (tmpSplit != null && tmpSplit.Length > 0)
                            {
                                foreach (var tmp in tmpSplit)
                                {
                                    si.ScheduleTaskWeeks.Add(tmp);
                                }

                            }
                        }

                        string specDay = dt.Rows[i]["SCHEDULETASKSPECIALDAYS"].ToString();

                        si.StrDays = specDay;

                        si.ScheduleTaskSpecialDays = new List<string>();
                        if (!string.IsNullOrWhiteSpace(specDay))
                        {
                            string[] tmpSplit = specDay.Split(',');

                            if (tmpSplit != null && tmpSplit.Length > 0)
                            {
                                foreach (var tmp in tmpSplit)
                                {
                                    si.ScheduleTaskSpecialDays.Add(tmp);
                                }

                            }
                        }

                        tasks.Add(si);
                    }

                }
            }


            return tasks;
        }



    }
}
