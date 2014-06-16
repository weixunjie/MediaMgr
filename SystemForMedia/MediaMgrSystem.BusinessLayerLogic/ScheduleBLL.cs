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
   


        public int RemoveSchedule(string id)
        {
            String sqlStr = "DELETE FROM SCHEDULEINFO WHERE SCHEDULEID=" + id;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddSchedule(ScheduleInfo si)
        {
            String sqlStr = "INSERT INTO SCHEDULEINFO(SCHEDULENAME,PPRGRAMID,SCHEDULETIME) values ('{0}','{1}','{2}')";

            sqlStr = String.Format(sqlStr, si.ScheduleName, si.ProgrameId,si.ScheduleTime);
            
            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateSchedule(ScheduleInfo si)
        {
            String sqlStr = "UPDATE SCHEDULEINFO SET SCHEDULENAME='{0}',PPRGRAMID='{1}',SCHEDULETIME='{2}' WHERE SCHEDULEID={3}";

            sqlStr = String.Format(sqlStr, si.ScheduleName, si.ProgrameId,si.ScheduleTime,si.ScheduleId);

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
                        si.ProgrameId = dt.Rows[i]["PPRGRAMID"].ToString();
                        si.ScheduleTime = dt.Rows[i]["SCHEDULETIME"].ToString();

                        sInfos.Add(si);
                    }

                }
            }

            return sInfos;

        }
    }
}
