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

        public List<GroupInfo> GetGroupByIds(string groupIds)
        {
            String sqlStr = "SELECT * FROM GROUPINFO WHERE GROUPID in (" + groupIds + ")";

            return GetGroupList(sqlStr);
        }

        public List<GroupInfo> GetGroupByChannelId(string id, BusinessType bType)
        {

            String sqlStr = "SELECT * FROM GROUPINFO WHERE CHANNELID=" + id;

            return GetGroupList(sqlStr, bType);
        }

        public List<GroupInfo> GetGroupByVideoEncoderId(string id, BusinessType bType)
        {

            String sqlStr = "SELECT * FROM GROUPINFO WHERE EcnoderId=" + id;

            return GetGroupList(sqlStr, bType);
        }

        public List<GroupInfo> GetAllGroups()
        {
            String sqlStr = "SELECT * FROM GroupInfo";

            return GetGroupList(sqlStr);

        }

        public List<GroupInfo> GetAllGroupsWithOutDeviceInfo()
        {
            String sqlStr = "SELECT * FROM GroupInfo";

            return GetGroupList(sqlStr,BusinessType.ALL,false);

        }

        public List<GroupInfo> GetAllGroupsByBusinessType(BusinessType bType)
        {
            String sqlStr = "SELECT * FROM GroupInfo";

            return GetGroupList(sqlStr, bType);

        }





        public int RemoveGroup(string goupId)
        {
            String sqlStr = "DELETE FROM GROUPINFO WHERE GROUPID=" + goupId;


            String strUpdateDeviceToDefaultGroup = "UPDATE DEVICEINFO SET GROUPID='-1' WHERE GROUPID=" + goupId;

            dbUitls.ExecuteNonQuery(sqlStr);

            return dbUitls.ExecuteNonQuery(strUpdateDeviceToDefaultGroup);

        }

        public string AddGroup(GroupInfo gi)
        {
            String sqlStr = "INSERT INTO GROUPINFO(GROUPNAME) values ('{0}') SELECT SCOPE_IDENTITY()";

            sqlStr = String.Format(sqlStr, gi.GroupName);

            string newId = dbUitls.ExecuteScalar(sqlStr).ToString();

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
                    String sqlStrUpdateDevice = "UPDATE DEVICEINFO SET GROUPID='-1' WHERE DEVICEID={0}";

                    sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, device.DeviceId);

                    dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);

                }
            }
        }

        public void UpdateGroupChannel(string groupId, string channelId)
        {

            String sqlStrUpdateDevice = "UPDATE GROUPINFO SET CHANNELID='{0}' WHERE GROUPID={1}";

            sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, channelId, groupId);

            dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);



        }
        public void UpdateGroupChannelAndEncoder(string groupId, string channelId, string encoderId)
        {

            String sqlStrUpdateDevice = "UPDATE GROUPINFO SET CHANNELID='{0}', ENCODERID='{1}' WHERE GROUPID={2}";

            sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, channelId, encoderId, groupId);

            dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);



        }


        public void UpdateGroupEncoder(string groupId, string encoderId)
        {

            String sqlStrUpdateDevice = "UPDATE GROUPINFO SET ENCODERID='{0}' WHERE GROUPID={1}";

            sqlStrUpdateDevice = String.Format(sqlStrUpdateDevice, encoderId, groupId);

            dbUitls.ExecuteNonQuery(sqlStrUpdateDevice);



        }

        public int UpdateGroup(GroupInfo gi)
        {
            String sqlStr = "UPDATE GROUPINFO SET GROUPNAME='{0}' WHERE GROUPID={1}";

            UpdateDeviceGroup(gi);


            sqlStr = String.Format(sqlStr, gi.GroupName, gi.GroupId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<GroupInfo> GetGroupList(string sqlStr, BusinessType bType = BusinessType.ALL, bool isGeDevice=true)
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
                        gi.ChannelId = dt.Rows[i]["CHANNELID"].ToString();
                        gi.EncoderId = dt.Rows[i]["ENCODERID"].ToString();

                        if (isGeDevice)
                        {
                            if (bType == BusinessType.ALL)
                            {
                                gi.Devices = deviceBLL.GetAllDevicesByGroup(gi.GroupId);
                            }
                            else
                            {
                                gi.Devices = deviceBLL.GetAllDevicesByGroupWithFilter(gi.GroupId, bType);
                            }
                        }
                        groups.Add(gi);
                    }
                }
            }

            return groups;

        }
    }
}
