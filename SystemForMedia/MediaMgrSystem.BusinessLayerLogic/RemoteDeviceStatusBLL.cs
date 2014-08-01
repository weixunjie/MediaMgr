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
    public class RemoteDeviceStatusBLL
    {

        DbUtils dbUitls = null;


        public RemoteDeviceStatusBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public void RemoveAll()
        {
            String sqlStr = "DELETE  FROM REMOTECONTROLDEVICESTATUS";

            dbUitls.ExecuteNonQuery(sqlStr);
        }



        public List<RemoteDeviceStatus> GetAllStatus()
        {
            String sqlStr = "SELECT * FROM REMOTECONTROLDEVICESTATUS";

            List<RemoteDeviceStatus> result = GetAllClientList(sqlStr);


            return result;
        }

        public int AddStatus(RemoteDeviceStatus sc)
        {

            String sqlStr = "INSERT INTO REMOTECONTROLDEVICESTATUS(CLIENTIDENTIFY,DEVICETYPE,DEVICESTATUS,ACTEMPATURE,ACMODE) values ('{0}','{1}','{2}')";

            sqlStr = String.Format(sqlStr, sc.ClientIdentify, sc.DeviceType.ToString(), sc.DeviceOpenedStatus ? "1" : "0", sc.ACTempature, sc.ACMode);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int DeleteByClientIdentifyAndType(string strIdentify, RemoveControlDeviceType type)
        {

            String sqlStr = "DELETE FROM REMOTECONTROLDEVICESTATUS WHERE CLIENTIDENTIFY='{0}' AND DEVICETYPE='{1}'";

            sqlStr = String.Format(sqlStr, strIdentify,type.ToString());

            return dbUitls.ExecuteNonQuery(sqlStr);

       
        }


        protected List<RemoteDeviceStatus> GetAllClientList(string sqlStr)
        {
            List<RemoteDeviceStatus> sccs = new List<RemoteDeviceStatus>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RemoteDeviceStatus rdc = new RemoteDeviceStatus();
                        rdc.ClientIdentify = dt.Rows[i]["ClientIdentify"].ToString();
                       
                        string cType = dt.Rows[i]["DEVICETYPE"].ToString();

                        RemoveControlDeviceType type = RemoveControlDeviceType.NONE;

                        if (Enum.TryParse(cType, out type))
                        {
                            rdc.DeviceType = type;
                        }

                        rdc.DeviceOpenedStatus = dt.Rows[i]["DEVICESTATUS"].ToString()=="1";

                        rdc.ACTempature = dt.Rows[i]["ACTEMPATURE"].ToString();

                        rdc.ACMode = dt.Rows[i]["ACMODE"].ToString();

                        sccs.Add(rdc);
                    }

                }
            }

            return sccs;

        }



    }
}
