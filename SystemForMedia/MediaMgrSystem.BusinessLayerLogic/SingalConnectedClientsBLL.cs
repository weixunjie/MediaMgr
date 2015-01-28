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
    public class SingalConnectedClientsBLL
    {

        DbUtils dbUitls = null;


        public SingalConnectedClientsBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public List<SingalConnectedClient> GetSingalConnectedClientsByType(string connType)
        {
            String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE CONNECTIONTYPE='" + connType + "'";

            return GetAllClientList(sqlStr);
        }

        public void RemoveAll()
        {
            String sqlStr = "DELETE  FROM SINGALCONNECTEDCLIENTS";

            dbUitls.ExecuteNonQuery(sqlStr);
        }


        public List<SingalConnectedClient> GetSingalConnectedClientsByIndetifies(List<string> indetifies)
        {
            string sqlIndetifies = string.Empty;
            if (indetifies != null)
            {
                foreach (var str in indetifies)
                {
                    sqlIndetifies += "'" + str + "'" + ",";
 
                }
 
            }

            sqlIndetifies = sqlIndetifies.TrimEnd(',');

            String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE ConnectionIdentify IN (" + sqlIndetifies + ")";

            return GetAllClientList(sqlStr);
        }

        public string  GetSingalConnectedClientsByIndetify(string strIndetify,string scType)
        {
            String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE ConnectionIdentify ='" + strIndetify + "' AND CONNECTIONTYPE='"+scType+"'";

            List<SingalConnectedClient> scs= GetAllClientList(sqlStr);

            if (scs != null && scs.Count >= 1)
            {
                return scs[0].ConnectionId;
            }

            return string.Empty;
        }

        public List<string> GetSingalConnectedClientsByIndetifyAll(string strIndetify, string scType)
        {
            String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE ConnectionIdentify ='" + strIndetify + "' AND CONNECTIONTYPE='" + scType + "'";

            List<SingalConnectedClient> scs = GetAllClientList(sqlStr);

            List<string> r = new List<string>();
            if (scs != null && scs.Count >= 1)
            {
                foreach(var sc in scs)
                {
                    r.Add(sc.ConnectionId);
                }
               
            }

            return r;
        }



        public SingalConnectedClient GetSingalConnectedClientsById(string conntectId)
        {
            String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE CONNECTIONID='" + conntectId + "'";

            List<SingalConnectedClient> result= GetAllClientList(sqlStr);

            if (result != null && result.Count >= 1)
            {
                return result[0];
            }

            return null;
        }

        public int AddSingalConnectedClient(SingalConnectedClient sc)
        {

            String sqlStr = "INSERT INTO SINGALCONNECTEDCLIENTS(CONNECTIONID,CONNECTIONIDENTIFY,CONNECTIONTYPE) values ('{0}','{1}','{2}')";

            sqlStr = String.Format(sqlStr, sc.ConnectionId,sc.ConnectionIdentify,sc.ConnectionType.ToString());

            return dbUitls.ExecuteNonQuery(sqlStr);

            //String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE CONNECTIONID='" + conntectId + "'";

           // return GetAllClientList(sqlStr);
        }

        public int DeleteSingalConnectedClientByConnectIds(string ind)
        {

            String sqlStr = "DELETE FROM SINGALCONNECTEDCLIENTS WHERE CONNECTIONID='{0}'";

            sqlStr = String.Format(sqlStr, ind);

            return dbUitls.ExecuteNonQuery(sqlStr);

            //String sqlStr = "SELECT * FROM SINGALCONNECTEDCLIENTS WHERE CONNECTIONID='" + conntectId + "'";

            // return GetAllClientList(sqlStr);
        }


        protected List<SingalConnectedClient> GetAllClientList(string sqlStr)
        {

            List<SingalConnectedClient> sccs = new List<SingalConnectedClient>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        SingalConnectedClient si = new SingalConnectedClient();
                        si.ConnectionId = dt.Rows[i]["CONNECTIONID"].ToString();
                        si.ConnectionIdentify = dt.Rows[i]["CONNECTIONIDENTIFY"].ToString();
                        string cType = dt.Rows[i]["CONNECTIONTYPE"].ToString();

                        SingalRClientConnectionType type = SingalRClientConnectionType.PC;

                        if (Enum.TryParse(cType, out type))
                        {
                            si.ConnectionType = type;
                        }

                        sccs.Add(si);
                    }

                }
            }

            return sccs;

        }



    }
}
