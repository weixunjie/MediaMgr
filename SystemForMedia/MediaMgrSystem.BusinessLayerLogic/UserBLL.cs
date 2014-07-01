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
    public class UserBLL
    {
        DbUtils dbUitls = null;
        public UserBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public UserInfo GetUserByCritiea(string userCode, string password)
        {
            String sqlStr = "SELECT * FROM USERINFO WHERE USERCODE='{0}' AND PASSWORD='{1}' AND ISACTIVE='1'";

            sqlStr = string.Format(sqlStr, userCode, password);

            List<UserInfo> users = GetUserList(sqlStr);
            if (users != null && users.Count() > 0)
            {
                return users[0];
            }
            else
            {
                return null;
            }

        }

        public UserInfo GetUserById(string userID)
        {
            String sqlStr = "SELECT * FROM USERINFO WHERE USERID='{0}' AND USERCODE<>'Admin'";

            sqlStr = string.Format(sqlStr, userID);

            List<UserInfo> users = GetUserList(sqlStr);
            if (users != null && users.Count() > 0)
            {
                return users[0];
            }
            else
            {
                return null;
            }

        }

       
        public List<UserInfo> GetAllUses()
        {

            String sqlStr = "SELECT * FROM USERINFO WHERE USERCODE<>'Admin'";

            return GetUserList(sqlStr);

        }

   
        public int RemoveUser(string channelId)
        {
            String sqlStr = "DELETE FROM USERINFO WHERE  USERID=" + channelId+" AND USERCODE<>'Admin'";

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddUser(UserInfo ui)
        {

            string sqlStr = "INSERT INTO USERINFO(USERCODE,USERNAME,USERLEVEL,ISACTIVE,PASSWORD) VALUES ('{0}','{1}','{2}','1','{3}')";

            sqlStr = String.Format(sqlStr, ui.UserCode, ui.UserName, ui.UserLevel,ui.Password);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateUser(UserInfo ui)
        {
            String sqlStr = "UPDATE USERINFO SET USERCODE='{0}',USERNAME='{1}', USERLEVEL='{2}',ISACTIVE='{3}',PASSWORD='{4}' WHERE USERID={5}";

            sqlStr = String.Format(sqlStr, ui.UserCode, ui.UserName, ui.UserLevel,ui.IsActive?"1":"0", ui.Password,ui.UserId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

    

        protected List<UserInfo> GetUserList(string sqlStr)
        {
            List<UserInfo> users = new List<UserInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        UserInfo ui = new UserInfo();
                        ui.UserId = dt.Rows[i]["USERID"].ToString();

                        ui.UserName = dt.Rows[i]["USERNAME"].ToString();

                        ui.UserLevel = dt.Rows[i]["USERLEVEL"].ToString();

                        ui.Password = dt.Rows[i]["PASSWORD"].ToString();

                        ui.UserCode = dt.Rows[i]["UserCode"].ToString();

                        string isActive=dt.Rows[i]["IsActive"].ToString();


                        ui.IsActive = false;

                        if (!string.IsNullOrWhiteSpace(isActive))
                        {
                            ui.IsActive = isActive == "1";
                        }
                    
                        users.Add(ui);
                    }
                }
            }

            return users;

        }
    }
}
