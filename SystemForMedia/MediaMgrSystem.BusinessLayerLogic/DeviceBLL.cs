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

        public List<DeviceInfo> GetAllDevicesByGroupWithFilter(string groupId, BusinessType bType)
        {
            String sqlStr = "SELECT * FROM DEVICEINFO WHERE GROUPID='" + groupId + "'";

            string strAnd = string.Empty; ;
            switch (bType)
            {
                case BusinessType.AUDITBROADCAST:
                    strAnd = " AND ISUSEDFORAUDIO=1";
                    break;
                case BusinessType.VIDEOONLINE:
                    strAnd = " AND ISUSEDFORENCODER=1";
                    break;

                case BusinessType.REMOVECONTROL:
                    strAnd = " AND ISUSEDFORREMOTECONTROL=1";
                    break;

            }

            return GetDeviceList(sqlStr + strAnd);

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

            //if (GetDevicesCount() >= pc.MaxClientsCount)
            //{
            //    logBLL.AddLog("系统提示", "终端已经达到最大数量");

            //    return -1;
            //}

           
                String sqlStr = "INSERT INTO DEVICEINFO(DEVICENAME,DEVICEIPADDRESS,GROUPID,ISUSEDFORAUDIO,ISUSEDFORENCODER,ISUSEDFORREMOTECONTROL) values ('{0}','{1}','{2}','{3}','{4}','{5}')";

                sqlStr = String.Format(sqlStr, di.DeviceName, di.DeviceIpAddress, di.GroupId, di.UsedToAudioBroandcast ? 1 : 0, di.UsedToVideoOnline ? 1 : 0, di.UsedToRemoteControl ? 1 : 0);


                return dbUitls.ExecuteNonQuery(sqlStr);
        //    }




        }

        public int UpdateDevice(DeviceInfo di)
        {

            String sqlStr = "UPDATE DEVICEINFO SET DEVICENAME='{0}',DEVICEIPADDRESS='{1}',GROUPID='{2}',ISUSEDFORAUDIO='{3}',ISUSEDFORENCODER='{4}', ISUSEDFORREMOTECONTROL='{5}' WHERE DEVICEID={6}";

            sqlStr = String.Format(sqlStr, di.DeviceName, di.DeviceIpAddress, di.GroupId, di.UsedToAudioBroandcast ? 1 : 0, di.UsedToVideoOnline ? 1 : 0, di.UsedToRemoteControl ? 1 : 0, di.DeviceId);

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


                        di.UsedToAudioBroandcast = dt.Rows[i]["ISUSEDFORAUDIO"].ToString() == "1";

                        di.UsedToVideoOnline = dt.Rows[i]["ISUSEDFORENCODER"].ToString() == "1";

                        di.UsedToRemoteControl = dt.Rows[i]["ISUSEDFORREMOTECONTROL"].ToString() == "1";

                        deviceInfos.Add(di);
                    }

                }
            }

            return deviceInfos;

        }
    }
}
