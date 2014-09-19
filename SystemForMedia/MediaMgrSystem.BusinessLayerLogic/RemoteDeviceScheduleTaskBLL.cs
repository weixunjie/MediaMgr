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
    public class RemoteDeviceScheduleTaskBLL
    {

        DbUtils dbUitls = null;


        public RemoteDeviceScheduleTaskBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public void RemoveAll()
        {
            String sqlStr = "DELETE  FROM REMOTECONTROLSCHEDULETASK";

            dbUitls.ExecuteNonQuery(sqlStr);
        }



        public List<RemoteDeviceScheduleTask> GetAllTask()
        {
            String sqlStr = "SELECT * FROM REMOTECONTROLSCHEDULETASK";

            List<RemoteDeviceScheduleTask> result = GetAllTaskList(sqlStr);


            return result;
        }

        public int AddTask(RemoteDeviceScheduleTask sc)
        {

            String sqlStr = "INSERT INTO REMOTECONTROLSCHEDULETASK(CLIENTIDENTIFY,DEVICETYPE,NEWDEVICESTATUS,ACTEMPATURE,ACMODE,WEEKS,TASKTIME) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";

            sqlStr = String.Format(sqlStr, sc.ClientIdentify, sc.DeviceType, sc.DeviceOpenedStatus ? "1" : "0", sc.ACTempature, sc.ACMode, sc.Weeks, sc.TaskTime);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int DeleteByIdentifyAndType(string strIdentify, string type, string isOpen)
        {

            String sqlStr = "DELETE FROM REMOTECONTROLSCHEDULETASK WHERE CLIENTIDENTIFY='{0}' AND DEVICETYPE='{1}' AND NEWDEVICESTATUS='{2}' ";

            sqlStr = String.Format(sqlStr, strIdentify, type, isOpen);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        protected List<RemoteDeviceScheduleTask> GetAllTaskList(string sqlStr)
        {
            List<RemoteDeviceScheduleTask> sccs = new List<RemoteDeviceScheduleTask>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RemoteDeviceScheduleTask rdc = new RemoteDeviceScheduleTask();
                        rdc.ClientIdentify = dt.Rows[i]["CLIENTIDENTIFY"].ToString();

                        string cType = dt.Rows[i]["DEVICETYPE"].ToString();


                        rdc.DeviceType = cType;


                        rdc.DeviceOpenedStatus = dt.Rows[i]["NEWDEVICESTATUS"].ToString() == "1";

                        rdc.ACTempature = dt.Rows[i]["ACTEMPATURE"].ToString();

                        rdc.ACMode = dt.Rows[i]["ACMODE"].ToString();

                        rdc.Weeks = dt.Rows[i]["WEEKS"].ToString();

                        rdc.TaskTime = dt.Rows[i]["TASKTIME"].ToString();

                        sccs.Add(rdc);
                    }

                }
            }

            return sccs;

        }



    }
}
