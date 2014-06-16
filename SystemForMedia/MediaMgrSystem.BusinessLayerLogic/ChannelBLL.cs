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
    public class ChannelBLL
    {
        DbUtils dbUitls = null;
        public ChannelBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public ChannelInfo GetChannelById(string channelId)
        {

            String sqlStr = "SELECT * FROM CHANNELINFO WHERE CHANNELID=" + channelId;

            List<ChannelInfo> cis = GetChannelList(sqlStr);
            if (cis != null && channelId.Count() > 0)
            {
                return cis[0];
            }
            else
            {
                return null;
            }

        }

        public ChannelInfo GetChannelByScheduleId(string sid)
        {

            String sqlStr = "SELECT * FROM CHANNELINFO WHERE ScheduelId=" + sid;

            List<ChannelInfo> cis = GetChannelList(sqlStr);
            if (cis != null && cis.Count() > 0)
            {
                return cis[0];
            }
            else
            {
                return null;
            }

        }

        public List<ChannelInfo> GetAllChannels()
        {

            String sqlStr = "SELECT * FROM CHANNELINFO";

            return GetChannelList(sqlStr);

        }

        public int RemoveChannel(string channelId)
        {
            String sqlStr = "DELETE FROM CHANNELINFO WHERE CHANNELID=" + channelId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddChannel(ChannelInfo ci)
        {       

            String sqlStr = "INSERT INTO CHANNELINFO(CHANNELNAME) values ('{0}')";

            sqlStr = String.Format(sqlStr,ci.ChannelName);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateChannel(ChannelInfo ci)
        {
            String sqlStr = "UPDATE CHANNELINFO SET CHANNELNAME='{0}' WHERE CHANNELID={1}";

            sqlStr = String.Format(sqlStr, ci.ChannelName,ci.ChannelId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateChannelSchedule(string cid, string sid)
        {
            String sqlStr = "UPDATE CHANNELINFO SET SCHEDUELID='{0}' WHERE CHANNELID={1}";

            sqlStr = String.Format(sqlStr, sid, cid);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<ChannelInfo> GetChannelList(string sqlStr)
        {
            List<ChannelInfo> channelsInfo = new List<ChannelInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ChannelInfo ci = new ChannelInfo();
                        ci.ChannelId = dt.Rows[i]["CHANNELID"].ToString();
                        ci.ChannelName = dt.Rows[i]["CHANNELNAME"].ToString();
                        ci.ScheduelId = dt.Rows[i]["SCHEDUELID"].ToString();
                        channelsInfo.Add(ci);
                    }
                }
            }

            return channelsInfo;

        }
    }
}
