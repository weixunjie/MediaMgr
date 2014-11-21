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
    public class LogSignalRConnectionBLL
    {
        DbUtils dbUitls = null;
        public LogSignalRConnectionBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

   

        
        public int AddLog(string logName, string logDesp)
        {
            String sqlStr = "INSERT INTO LOGINFOFORSINGALRCONNECTION(LOGNAME,LOGDESP,LOGDATE) values ('{0}','{1}',getdate())";

            sqlStr = String.Format(sqlStr, logName, logDesp);

            return dbUitls.ExecuteNonQuery(sqlStr);
        }

       
    }
}
