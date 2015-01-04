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
    public class VideoEncoderBLL
    {
        DbUtils dbUitls = null;
        public VideoEncoderBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }



        public VideoEncoderInfo GetEncoderById(string encoderId)
        {
            String sqlStr = "SELECT * FROM VIDEOENCODERINFO WHERE ENCODERID=" + encoderId;

            List<VideoEncoderInfo> eis = GetEncoderList(sqlStr);
            if (eis != null && eis.Count() > 0)
            {
                return eis[0];
            }
            else
            {
                return null;
            }

        }
 

        public List<VideoEncoderInfo> GetAllEncoders()
        {

            String sqlStr = "SELECT * FROM VIDEOENCODERINFO";

            return GetEncoderList(sqlStr);

        }

        public int RemoveEncoder(string encoderId)
        {
            String sqlStr = "DELETE FROM VIDEOENCODERINFO WHERE ENCODERID=" + encoderId;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddEncoder(VideoEncoderInfo ei)
        {

        
            String sqlStr = "INSERT INTO VIDEOENCODERINFO(ENCODERNAME,BAUDRATE,UDPADDRESS) VALUES ('{0}','{1}','{2}')";

            sqlStr = String.Format(sqlStr, ei.EncoderName,ei.BaudRate,ei.UdpAddress);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int UpdateEncoder(VideoEncoderInfo ei)
        {
            String sqlStr = "UPDATE VIDEOENCODERINFO SET ENCODERNAME='{0}',BAUDRATE='{1}', UDPADDRESS='{2}'  WHERE ENCODERID={3}";

            sqlStr = String.Format(sqlStr, ei.EncoderName, ei.BaudRate, ei.UdpAddress, ei.EncoderId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<VideoEncoderInfo> GetEncoderList(string sqlStr)
        {
            List<VideoEncoderInfo> encoderInfo = new List<VideoEncoderInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        VideoEncoderInfo ei = new VideoEncoderInfo();
                        ei.EncoderId = dt.Rows[i]["ENCODERID"].ToString();
                        ei.EncoderName = dt.Rows[i]["ENCODERNAME"].ToString();
                        ei.BaudRate = dt.Rows[i]["BAUDRATE"].ToString();
                        ei.UdpAddress = dt.Rows[i]["UDPADDRESS"].ToString();
                        encoderInfo.Add(ei);
                    }
                }
            }

            return encoderInfo;

        }
    }
}
