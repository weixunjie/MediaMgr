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
    public class LogBLL
    {
        DbUtils dbUitls = null;
        public LogBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public List<LogInfo> GetTop3Logs()
        {

            String sqlStr = "SELECT TOP 3 * FROM LOGINFO ORDER by LOGDATE DESC";

            return GetLogList(sqlStr);

        }


        public List<LogInfo> GetLogsByCriteria(string name, string dataBefore, string dateAfter)
        {

            String sqlStr = "SELECT  * FROM LOGINFO WHERE LOGNAME LIKE '%{0}%' and LOGDATE>='{1}' AND LOGDATE<='{2}'  ORDER by LOGDATE DESC ";

            sqlStr = string.Format(sqlStr, name, dataBefore + " 00:00:00", dateAfter + " 23:59:59");
            return GetLogList(sqlStr);

        }

        public int RemoveLog(string logId)
        {
            String sqlStr = "DELETE FROM LOGINFO where LOGID=" + logId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int RemoveLogByDayBefore(string strDayBefore)
        {
            if (strDayBefore == "0")
            {
                ///delete all
                return dbUitls.ExecuteNonQuery("DELETE FROM LOGINFO");
            }

            int intDayBefore;
            if (int.TryParse(strDayBefore, out intDayBefore))
            {
                String sqlStr = "DELETE FROM LOGINFO WHERE LOGDATE<='{0}'";

                sqlStr = String.Format(sqlStr, DateTime.Now.AddDays(-intDayBefore).ToString("yyyy-MM-dd 00:00:00"));

                return dbUitls.ExecuteNonQuery(sqlStr);
            }

            return 1;
        }

        public int AddLog(string logName, string logDesp)
        {
            String sqlStr = "INSERT INTO LOGINFO(LOGNAME,LOGDESP,LOGDATE) values ('{0}','{1}',getdate())";

            sqlStr = String.Format(sqlStr, logName, logDesp);

            return dbUitls.ExecuteNonQuery(sqlStr);
        }

        protected List<LogInfo> GetLogList(string sqlStr)
        {
            List<LogInfo> logInfos = new List<LogInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        LogInfo li = new LogInfo();
                        li.LogId = dt.Rows[i]["LOGID"].ToString();
                        li.LogName = dt.Rows[i]["LOGNAME"].ToString();
                        li.LogDesp = dt.Rows[i]["LOGDESP"].ToString();
                        string strDate = dt.Rows[i]["LOGDATE"].ToString();
                        DateTime dtOut;
                        if (DateTime.TryParse(strDate, out dtOut))
                        {
                            li.LogDate = dtOut.ToString("yyyy-MM-dd HH:mm:ss");

                        }
                        logInfos.Add(li);
                    }
                }
            }

            return logInfos;

        }
    }
}
