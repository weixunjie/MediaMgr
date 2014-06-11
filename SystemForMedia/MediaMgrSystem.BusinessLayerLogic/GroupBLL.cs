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
    public class GroupBLL
    {
        private DeviceBLL deviceBLL;
        DbUtils dbUitls = null;
        public GroupBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
            deviceBLL = new DeviceBLL(dbUitls);
        }

        public List<GroupInfo> GetGroupById(string groupId)
        {
            String sqlStr = "SELECT * FROM GROUPINFO WHERE GROUPID=" + groupId;

            return GetGroupList(sqlStr);
        }

        public List<GroupInfo> GetGroupByChannelId(string id)
        {
            String sqlStr = "SELECT * FROM GROUPINFO WHERE CHANNELID=" + id;

            return GetGroupList(sqlStr);
        }

        public List<GroupInfo> GetAllGroups()
        {

            String sqlStr = "SELECT * FROM GroupInfo";

            return GetGroupList(sqlStr);

        }

        public int RemoveGroup(string goupId)
        {
            String sqlStr = "DELETE FROM GROUPINFO WHERE GROUPID=" + goupId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public string AddGroup(GroupInfo gi)
        {
            String sqlStr = "INSERT INTO GROUPINFO(GROUPNAME) values ('{0}') SELECT SCOPE_IDENTITY()";

            sqlStr = String.Format(sqlStr, gi.GroupName);
          
            string newId= dbUitls.ExecuteScalar(sqlStr).ToString();

            gi.GroupId = newId;
            UpdateDeviceGroup(gi);

            return newId;

        }

        private void UpdateDeviceGroup(GroupInfo gi)
        {
            if (gi.Devices != null)
            {
                foreach (var device in gi.Devices)
                {
                    String sqlStrUpdateDevice = "UPDATE DEVICEINFO SET GROUPID='{0}' WHERE DEVICEID={1}";

                    sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, gi.GroupId, device.DeviceId);

                    dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);

                }
            }
        }

        public void UpdateDeviceEmptyGroup(GroupInfo gi)
        {
            if (gi.Devices != null)
            {
                foreach (var device in gi.Devices)
                {
                    String sqlStrUpdateDevice = "UPDATE DEVICEINFO SET GROUPID='-1' WHERE DEVICEID={1}";

                    sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, device.DeviceId);

                    dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);

                }
            }
        }

        public int UpdateGroup(GroupInfo gi)
        {
            String sqlStr = "UPDATE GROUPINFO SET GROUPNAME='{0}' WHERE GROUPID={1}";

            UpdateDeviceGroup(gi);
      

            sqlStr = String.Format(sqlStr, gi.GroupName, gi.GroupId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<GroupInfo> GetGroupList(string sqlStr)
        {
            List<GroupInfo> groups = new List<GroupInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        GroupInfo gi = new GroupInfo();
                        gi.GroupId = dt.Rows[i]["GROUPID"].ToString();
                        gi.GroupName = dt.Rows[i]["GROUPNAME"].ToString();
                        gi.Devices = deviceBLL.GetAllDevicesByGroup(gi.GroupId);
                        groups.Add(gi);
                    }
                }
            }

            return groups;

        }
    }
}
