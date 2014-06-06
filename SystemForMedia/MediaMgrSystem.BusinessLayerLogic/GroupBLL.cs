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
            String sqlStr = "SELECT * FROM GROUPINFO FROM GROUPID=" + groupId;

            return GetGroupList(sqlStr);
        }

        public List<GroupInfo> GetAllDevices()
        {

            String sqlStr = "SELECT * FROM GroupInfo";

            return GetGroupList(sqlStr);

        }

        public int RemoveGroup(string goupId)
        {
            String sqlStr = "DELETE FROM GROUPINFO where GROUPID=" + goupId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddGroup(GroupInfo gi)
        {
            String sqlStr = "INSERT INTO GROUPINFO(GROUPNAME) values ('{0}')";

            sqlStr = String.Format(sqlStr, gi.GroupName);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateGroup(GroupInfo gi)
        {
            String sqlStr = "UPDATE GROUPINFO SET GROUPNAME='{0}' WHERE GROUPID={1}";

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
                        gi.GroupName = dt.Rows[i]["u"].ToString();
                        gi.Devices = deviceBLL.GetAllDevicesByGroup(gi.GroupId);
                        groups.Add(gi);
                    }
                }
            }

            return groups;

        }
    }
}
