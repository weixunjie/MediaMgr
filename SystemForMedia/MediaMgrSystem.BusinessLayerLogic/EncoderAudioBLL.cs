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
    public class EncoderAudioBLL
    {
        DbUtils dbUitls = null;
        public EncoderAudioBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public EncoderAudioInfo GetEncoderById(string encoderId)
        {
            String sqlStr = "SELECT * FROM ENCODERINFO WHERE ENCODERID=" + encoderId;

            List<EncoderAudioInfo> eis = GetEncoderList(sqlStr);
            if (eis != null && eis.Count() > 0)
            {
                return eis[0];
            }
            else
            {
                return null;
            }

        }

        public EncoderAudioInfo GetEncoderByClientIdentify(string clientIdentify)
        {
            String sqlStr = "SELECT * FROM ENCODERINFO WHERE ClientIdentify='" + clientIdentify + "'";

            List<EncoderAudioInfo> eis = GetEncoderList(sqlStr);
            if (eis != null && eis.Count() > 0)
            {
                return eis[0];
            }
            else
            {
                return null;
            }

        }

        public List<EncoderAudioInfo> GetAllEncoders()
        {

            String sqlStr = "SELECT * FROM ENCODERINFO";

            return GetEncoderList(sqlStr);

        }

        public int RemoveEncoder(string encoderId)
        {
            String sqlStr = "DELETE FROM ENCODERINFO WHERE ENCODERID=" + encoderId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddEncoder(EncoderAudioInfo ei)
        {
            String sqlStr = "INSERT INTO ENCODERINFO(ENCODERNAME,CLIENTIDENTIFY,BAUDRATE,Priority) values ('{0}','{1}','{2}','1')";

            sqlStr = String.Format(sqlStr, ei.EncoderName,ei.ClientIdentify,ei.BaudRate);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateEncoder(EncoderAudioInfo ei)
        {
            String sqlStr = "UPDATE ENCODERINFO SET ENCODERNAME='{0}',PRIORITY='{1}', BAUDRATE='{2}'  WHERE ENCODERID={3}";

            sqlStr = String.Format(sqlStr, ei.EncoderName, ei.Priority, ei.BaudRate, ei.EncoderId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<EncoderAudioInfo> GetEncoderList(string sqlStr)
        {
            List<EncoderAudioInfo> encoderInfo = new List<EncoderAudioInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        EncoderAudioInfo ei = new EncoderAudioInfo();
                        ei.EncoderId = dt.Rows[i]["ENCODERID"].ToString();
                        ei.EncoderName = dt.Rows[i]["ENCODERNAME"].ToString();
                        ei.BaudRate = dt.Rows[i]["BAUDRATE"].ToString();
                        ei.Priority = dt.Rows[i]["Priority"].ToString();
                        ei.ClientIdentify = dt.Rows[i]["CLIENTIDENTIFY"].ToString();
                        encoderInfo.Add(ei);
                    }
                }
            }

            return encoderInfo;

        }
    }
}
