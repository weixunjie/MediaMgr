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
    public class VolumnMappingBLL
    {
        DbUtils dbUitls = null;
        public VolumnMappingBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public string GetVolumnValueByIpAddress(string ipAddress)
        {
            String sqlStr = "SELECT  * FROM VolumnMapping where IpAddress='"+ipAddress+"'";

            List<VolumnInfo> lists= GetList(sqlStr);

            if (lists != null && lists.Count > 0)
            {
                return lists[0].VolumnValue;
            }
            else
            {
                return "15";
            }

        }

        public void UpdateVolValueByIpAddress(string ipAddress,string newVolValue)
        {

            String sqlStr = "SELECT  * FROM VolumnMapping where IpAddress='" + ipAddress + "'";

            List<VolumnInfo> lists = GetList(sqlStr);

            string sqlStrOper = string.Empty;

            if (lists != null && lists.Count > 0)
            {
                sqlStrOper = "Update  VolumnMapping  set VolumnValue='" + newVolValue + "' where IpAddress='" + ipAddress + "'";
            }
            else 
            {
                sqlStrOper = "Insert into  VolumnMapping  (VolumnValue,IpAddress) values ('" + newVolValue + "','" + ipAddress + "')";
            }

            dbUitls.ExecuteNonQuery(sqlStrOper);
          

        }
   

       

        protected List<VolumnInfo> GetList(string sqlStr)
        {
            List<VolumnInfo> volumnInfos = new List<VolumnInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        VolumnInfo li = new VolumnInfo();
                        li.IpAddress = dt.Rows[i]["IpAddress"].ToString();
                        li.VolumnValue = dt.Rows[i]["VolumnValue"].ToString();
                    
                        volumnInfos.Add(li);
                    }
                }
            }

            return volumnInfos;

        }
    }
}
