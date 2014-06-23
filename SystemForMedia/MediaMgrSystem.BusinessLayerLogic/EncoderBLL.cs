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

        public List<EncoderInfo> GetAllEncoders()
        {

            String sqlStr = "SELECT * FROM ENCODERINFO";

            return GetEncoderList(sqlStr);

        }

        public int RemoveEncoder(string encoderId)
        {
            String sqlStr = "DELETE FROM ENCODERINFO where ENCODERID=" + encoderId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddEncoder(EncoderInfo ei)
        {
            String sqlStr = "INSERT INTO ENCODERINFO(ENCODERNAME) values ('{0}')";

            sqlStr = String.Format(sqlStr,ei.EncoderName);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateEncoder(EncoderInfo ei)
        {
            String sqlStr = "UPDATE ENCODERINFO SET ENCODERNAME='{0}' WHERE ENCODERID={1}";

            sqlStr = String.Format(sqlStr, ei.EncoderName,ei.EncoderId);

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
