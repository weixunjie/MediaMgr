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
                   string tmpIntervalTimeFromStopToPlay = dt.Rows[0]["IntervalTimeFromStopToPlay"].ToString();
                   string tmpMaxClientsCountForVideo = dt.Rows[0]["MaxClientsCountForVideo"].ToString();

                   string tmpMaxClientsCountForAudio = dt.Rows[0]["MaxClientsCountForAudio"].ToString();

                   string tmpMaxClientsCountForRemoteControl = dt.Rows[0]["MaxClientsCountForRemoteControl"].ToString();

                    int tmp;


                    if (!string.IsNullOrWhiteSpace(tmpMaxClientsCountForVideo))
                    {
                        try
                        {
                            string plainMaxClientCount = EncryptUtils.DesDecrypt(tmpMaxClientsCountForVideo);


                            if (int.TryParse(plainMaxClientCount, out tmp))
                            {

                                pc.MaxClientsCountForVideo = tmp;

                            }


                        }
                        catch (Exception ex)
                        {
                            pc.MaxClientsCountForVideo = 10;
                        }
                    }



                    if (!string.IsNullOrWhiteSpace(tmpMaxClientsCountForAudio))
                    {
                        try
                        {
                            string plainMaxClientCount = EncryptUtils.DesDecrypt(tmpMaxClientsCountForAudio);


                            if (int.TryParse(plainMaxClientCount, out tmp))
                            {

                                pc.MaxClientsCountForAudio = tmp;

                            }


                        }
                        catch (Exception ex)
                        {
                            pc.MaxClientsCountForAudio = 10;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(tmpMaxClientsCountForRemoteControl))
                    {
                        try
                        {
                            string plainMaxClientCount = EncryptUtils.DesDecrypt(tmpMaxClientsCountForRemoteControl);


                            if (int.TryParse(plainMaxClientCount, out tmp))
                            {

                                pc.MaxClientsCountForRemoteControl = tmp;

                            }


                        }
                        catch (Exception ex)
                        {
                            pc.MaxClientsCountForRemoteControl = 10;
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
                    if (int.TryParse(tmpIntervalTimeFromStopToPlay, out tmp))
                    {

                        pc.IntervalTimeFromStopToPlay = tmp;

                    }



                }
            }

            return pc;
        }
    }
}