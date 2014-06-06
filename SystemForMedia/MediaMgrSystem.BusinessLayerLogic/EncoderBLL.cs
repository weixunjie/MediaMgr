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
    public class EncoderBLL
    {
        DbUtils dbUitls = null;
        public EncoderBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public EncoderInfo GetEncoderById(string encoderId)
        {
            String sqlStr = "SELECT * FROM ENCODERINFO FROM ENCODERID=" + encoderId;

            List<EncoderInfo> eis = GetEncoderList(sqlStr);
            if (eis != null && eis.Count() > 0)
            {
                return eis[0];
            }
            else
            {
                return null;
            }

        }

        public List<EncoderInfo> GetAllChannels()
        {

            String sqlStr = "SELECT * FROM CHANNELINFO";

            return GetEncoderList(sqlStr);

        }

        public int RemoveChannel(string channelId)
        {
            String sqlStr = "DELETE FROM CHANNELINFO where CHANNELID=" + channelId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddChannel(ChannelInfo ci)
        {
            String sqlStr = "INSERT INTO CHANNELINFO(CHANNENAME) values ('{0}')";

            sqlStr = String.Format(sqlStr,ci.ChannelName);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateChannel(ChannelInfo ci)
        {
            String sqlStr = "UPDATE CHANNELINFO SET CHANNENAME='{0}' WHERE CHANNEID={1}";

            sqlStr = String.Format(sqlStr, ci.ChannelName,ci.ChannelId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<EncoderInfo> GetEncoderList(string sqlStr)
        {
            List<EncoderInfo> encoderInfo = new List<EncoderInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EncoderInfo ei = new EncoderInfo();
                        ei.EncoderId = dt.Rows[i]["ENCODERID"].ToString();
                        ei.EncoderName = dt.Rows[i]["ENCODERNAME"].ToString();
                        encoderInfo.Add(ei);
                    }
                }
            }

            return encoderInfo;

        }
    }
}
