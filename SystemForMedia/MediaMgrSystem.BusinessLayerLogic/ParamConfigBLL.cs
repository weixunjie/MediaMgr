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
    public class ParamConfigBLL
    {
        DbUtils dbUitls = null;
        public ParamConfigBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
        }

        public ParamConfig GetParamConfig()
        {
            String sqlStr = "SELECT TOP 1 * FROM PARAMCONFIG";


            ParamConfig pc = new ParamConfig();

            pc.BufferTimeForManualPlay = 4000;

            pc.BufferTimeForSchedule = 5000;


            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    string tmpBufferTimeForManualPlay = dt.Rows[0]["BufferTimeForManualPlay"].ToString();

                    string tmpBufferTimeForSchedule = dt.Rows[0]["BufferTimeForSchedule"].ToString();

                    string tmpMaxClientsCount = dt.Rows[0]["MaxClientsCount"].ToString();

                    int tmp;


                    if (!string.IsNullOrWhiteSpace(tmpMaxClientsCount))
                    {
                        try
                        {
                            string plainMaxClientCount = EncryptUtils.DesDecrypt(tmpMaxClientsCount);


                            if (int.TryParse(plainMaxClientCount, out tmp))
                            {

                                pc.MaxClientsCount = tmp;

                            }


                        }
                        catch (Exception ex)
                        {
                            pc.MaxClientsCount = 10;
                        }
                    }



                    if (int.TryParse(tmpBufferTimeForManualPlay, out tmp))
                    {

                        pc.BufferTimeForManualPlay = tmp;

                    }


                    if (int.TryParse(tmpBufferTimeForSchedule, out tmp))
                    {

                        pc.BufferTimeForSchedule = tmp;

                    }



                }
            }

            return pc;
        }
    }
}