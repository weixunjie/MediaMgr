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
    public class EncoderRunningClientsBLL
    {

        DbUtils dbUitls = null;


        public EncoderRunningClientsBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

       public List<RunningEncoder> GetAllEncoderRunning()
        {
            string sqlStr = "SELECT * FROM RUNNINGENCODER";


            List<RunningEncoder> res = new List<RunningEncoder>();

            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    RunningEncoder re = new RunningEncoder();
                    re.ClientIdentify = dt.Rows[i]["ClientIdentify"].ToString();
                    re.Priority = dt.Rows[i]["Priority"].ToString();
                    res.Add(re);

                }
            }

            return res;


        }
      



    }
}
