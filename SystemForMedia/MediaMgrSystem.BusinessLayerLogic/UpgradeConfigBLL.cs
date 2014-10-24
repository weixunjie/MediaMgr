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
    public class UpgradeConfigBLL
    {
        DbUtils dbUitls = null;
        public UpgradeConfigBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }


        public int UpdateUpgradeConfig(UpgradeInfo ui)
        {

            dbUitls.ExecuteNonQuery("DELETE FROM  UPGRADEINFO  WHERE VERSIONTYPE='" + ui.VersionType + "'");

            string sqlStrInsert = "INSERT INTO UPGRADEINFO(VERSIONTYPE,VersionId,UpgardeURL) values ('{0}','{1}','{2}')";

            sqlStrInsert = String.Format(sqlStrInsert, ui.VersionType, ui.VersionId, ui.UpgardeUrl);

            return dbUitls.ExecuteNonQuery(sqlStrInsert);

        }

        public UpgradeInfo GetUpgradeConfig(string verType)
        {
            String sqlStr = "SELECT  * FROM UPGRADEINFO WHERE VERSIONTYPE='" + verType + "'";


            UpgradeInfo ui = new UpgradeInfo();


            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {


                    ui.VersionId = dt.Rows[0]["VERSIONID"].ToString();

                    ui.UpgardeUrl = dt.Rows[0]["UpgardeURL"].ToString();
                }
            }

            return ui;
        }
    }
}