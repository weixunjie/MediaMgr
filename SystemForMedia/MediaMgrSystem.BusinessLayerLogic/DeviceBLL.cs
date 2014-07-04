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
    public class DeviceBLL
    {

        DbUtils dbUitls = null;

        LogBLL logBLL = null;

        ParamConfigBLL paramConfigBLL = null;

        public DeviceBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
            paramConfigBLL = new ParamConfigBLL(dbUitls);
            logBLL = new LogBLL(dbUitls);

        }

        public List<DeviceInfo> GetAllDevicesByGroup(string groupId)
        {
            String sqlStr = "SELECT * FROM DEVICEINFO WHERE GROUPID='" + groupId + "'";

            return GetDeviceList(sqlStr);

        }

        public List<DeviceInfo> GetADevicesById(string id)
        {

            String sqlStr = "SELECT * FROM DEVICEINFO WHERE DEVICEID='" + id + "'";

            return GetDeviceList(sqlStr);

        }

        public List<DeviceInfo> GetADevicesByIPAddress(string ipStr)
        {

            String sqlStr = "SELECT * FROM DEVICEINFO WHERE DEVICEIPADDRESS='" + ipStr + "'";

            return GetDeviceList(sqlStr);

        }


        public List<DeviceInfo> GetAllDevices()
        {

            String sqlStr = "SELECT * FROM DEVICEINFO";

            return GetDeviceList(sqlStr);

        }

        public int GetDevicesCount()
        {

            String sqlStr = "sELECT COUNT(DEVICEID) FROM DEVICEINFO";


            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0][0].ToString());
            }

            return 0;

        }

        public int RemoveDevice(string deviceId)
        {
            String sqlStr = "DELETE FROM DEVICEINFO WHERE DEVICEID=" + deviceId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddDevice(DeviceInfo di)
        {
            ParamConfig pc = paramConfigBLL.GetParamConfig();

            if (GetDevicesCount() >= pc.MaxClientsCount)
            {
                logBLL.AddLog("系统提示", "终端已经达到最大数量");

                return -1;
            }

            else
            {
                String sqlStr = "INSERT INTO DEVICEINFO(DEVICENAME,DEVICEIPADDRESS,GROUPID) values ('{0}','{1}','{2}')";

                sqlStr = String.Format(sqlStr, di.DeviceName, di.DeviceIpAddress, di.GroupId);


                return dbUitls.ExecuteNonQuery(sqlStr);
            }




        }

        public int UpdateDevice(DeviceInfo di)
        {
            String sqlStr = "UPDATE DEVICEINFO SET DEVICENAME='{0}',DEVICEIPADDRESS='{1}',GROUPID='{2}' WHERE DEVICEID={3}";

            sqlStr = String.Format(sqlStr, di.DeviceName, di.DeviceIpAddress, di.GroupId, di.DeviceId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateDeviceGroup(string deviceId, string groupdId)
        {
            String sqlStr = "UPDATE DEVICEINFO SET GROUPID='{0}' WHERE DEVICEID={1}";

            sqlStr = String.Format(sqlStr, groupdId, deviceId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }



        protected List<DeviceInfo> GetDeviceList(string sqlStr)
        {

            List<DeviceInfo> deviceInfos = new List<DeviceInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DeviceInfo di = new DeviceInfo();
                        di.DeviceId = dt.Rows[i]["DEVICEID"].ToString();
                        di.DeviceName = dt.Rows[i]["DEVICENAME"].ToString();
                        di.DeviceIpAddress = dt.Rows[i]["DEVICEIPADDRESS"].ToString();
                        di.GroupId = dt.Rows[i]["GROUPID"].ToString();


                        deviceInfos.Add(di);
                    }

                }
            }

            return deviceInfos;

        }
    }
}
