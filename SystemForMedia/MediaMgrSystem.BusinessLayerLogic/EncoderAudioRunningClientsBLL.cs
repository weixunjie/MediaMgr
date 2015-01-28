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
    public class EncoderAudioRunningClientsBLL
    {
        private DbUtils dbUitls;
        public EncoderAudioRunningClientsBLL(DbUtils _dbUitls)
        {
            dbUitls = _dbUitls;
        }

        public void UpdateRunningEncoder(RunningEncoder re)
        {
            string sqlStr = "DELETE FROM RUNNINGENCODER WHERE CLIENTIDENTIFY='" + re.ClientIdentify + "'";

            dbUitls.ExecuteNonQuery(sqlStr);

            sqlStr = "INSERT INTO  RUNNINGENCODER(CLIENTIDENTIFY,PRIORITY,GROUPIDS,DEVIDS) VALUES ('{0}','{1}','{2}','{3}')";

            dbUitls.ExecuteNonQuery(string.Format(sqlStr, re.ClientIdentify, re.Priority, re.GroupIds,re.DevIds));


        }

     
        public void RemoveRunningEncoder(string clientIdentify)
        {
            string sqlStr = "DELETE FROM RUNNINGENCODER WHERE CLIENTIDENTIFY='" + clientIdentify + "'";

            dbUitls.ExecuteNonQuery(sqlStr);

        }

        public void RemoveAllRunningEncoder()
        {
            string sqlStr = "DELETE FROM RUNNINGENCODER ";

            dbUitls.ExecuteNonQuery(sqlStr);

        }


        public EncoderAudioInfo GetEncoderList(string clientIdentify)
        {
            string sqlStr = "SELECT * from EncoderInfo where clientIdentify='" + clientIdentify + "'";
            EncoderAudioInfo ei = new EncoderAudioInfo();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        ei.EncoderId = dt.Rows[i]["ENCODERID"].ToString();
                        ei.EncoderName = dt.Rows[i]["ENCODERNAME"].ToString();
                        ei.ClientIdentify = dt.Rows[i]["CLIENTIDENTIFY"].ToString();
                        ei.BaudRate = dt.Rows[i]["BAUDRATE"].ToString();
                        ei.Priority = dt.Rows[i]["PRIORITY"].ToString();

                        break;
                    }
                }
            }

            return ei;

        }



        public List<RunningEncoder> GetAllEncoderRunning()
        {
            string sqlStr = "select * from RunningEncoder";

            List<RunningEncoder> res = new List<RunningEncoder>();

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RunningEncoder re = new RunningEncoder();
                    re.ClientIdentify = dt.Rows[i]["ClientIdentify"].ToString();
                    re.Priority = dt.Rows[i]["Priority"].ToString();
                    re.GroupIds = dt.Rows[i]["GroupIds"].ToString();
                    re.DevIds=dt.Rows[i]["DevIds"].ToString();
                    res.Add(re);
                }
            }

            return res;


        }

        public RunningEncoder CheckIfEncoderRunning(string clientIdentify)
        {
            string sqlStr = "select * from RunningEncoder where clientIdentify='" + clientIdentify + "'";

            RunningEncoder re = new RunningEncoder();
            
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    re.ClientIdentify = dt.Rows[i]["ClientIdentify"].ToString();
                    re.Priority = dt.Rows[i]["Priority"].ToString();
                    re.GroupIds = dt.Rows[i]["GroupIds"].ToString();
                    re.DevIds = dt.Rows[i]["DevIds"].ToString();

                    break;
                }
            }

            return re;


        }

    }
}
