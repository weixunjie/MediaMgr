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
    public class ProgramBLL
    {

        DbUtils dbUitls = null;
        public ProgramBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public List<ProgramInfo> GetProgramById(string pid)
        {
            String sqlStr = "SELECT * FROM PROGRAMINFO WHERE PROGRAMID=" + pid;

            return GetProgramList(sqlStr);
        }

        public List<ProgramInfo> GetAllProgramBy()
        {

            String sqlStr = "SELECT * FROM PROGRAMINFO";

            return GetProgramList(sqlStr);

        }



        public int RemoveProgram(string pid)
        {
            String sqlStr = "DELETE FROM PROGRAMINFO WHERE PROGRAMID=" + pid;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddPrograme(ProgramInfo pi)
        {
            String sqlStr = "INSERT INTO PROGRAMINFO(PROGRAMNAME,MAPPINGFILES,FILESBITRATE) values ('{0}','{1}','{2}')";
            string mapFiles;
            string mapBitRate;
            GetMappingValues(pi, out mapFiles, out mapBitRate);

            sqlStr = String.Format(sqlStr, pi.ProgramName,mapFiles,mapBitRate);

            int result = dbUitls.ExecuteNonQuery(sqlStr);

 
 

            return result;

        }

        private void GetMappingValues(ProgramInfo pi, out string mapFiles, out string mapBitRate)
        {
            mapFiles = string.Empty;
            mapBitRate = string.Empty;
            if (pi.MappingFiles != null)
            {
                foreach (var fa in pi.MappingFiles)
                {
                    mapFiles += fa.FileName + "|";
                    mapBitRate += fa.BitRate + "|";
                }

                mapFiles = mapFiles.TrimEnd('|');
                mapBitRate = mapBitRate.TrimEnd('|');
            }
        }


        public int UpdateProgram(ProgramInfo pi)
        {
            String sqlStr = "UPDATE PROGRAMINFO SET PROGRAMNAME='{0}',MAPPINGFILES='{1}' FILESBITRATE='{2}' WHERE PROGRAMID={3}";

            string mapFiles;
            string mapBitRate;
            GetMappingValues(pi, out mapFiles, out mapBitRate);
                
            sqlStr = String.Format(sqlStr, pi.ProgramName, mapFiles, mapBitRate,pi.ProgramId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<ProgramInfo> GetProgramList(string sqlStr)
        {
            List<ProgramInfo> groups = new List<ProgramInfo>();
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ProgramInfo pi = new ProgramInfo();
                        pi.ProgramId = dt.Rows[i]["PROGRAMID"].ToString();

                        pi.ProgramName = dt.Rows[i]["PROGRAMNAME"].ToString();

                        string mappingFiles = dt.Rows[i]["MappingFiles"].ToString();

                        string mappingBitRate = dt.Rows[i]["FilesBitRate"].ToString();

                        if (!string.IsNullOrEmpty(mappingFiles) && !string.IsNullOrEmpty(mappingBitRate))
                        {
                            string[] tmpMfile = mappingFiles.Split('|');

                            string[] tmpBitRate = mappingBitRate.Split('|');

                            pi.MappingFiles = new List<FileAttribute>();
                            for (int k = 0; k < tmpMfile.Length; k++)
                            {
                                FileAttribute fa = new FileAttribute();
                                fa.BitRate = tmpBitRate[k];
                                fa.FileName = tmpMfile[k];
                                
                            }

                        }

                        //gi.GroupId = dt.Rows[i]["GROUPID"].ToString();
                        //gi.GroupName = dt.Rows[i]["GROUPNAME"].ToString();
                        //gi.Devices = deviceBLL.GetAllDevicesByGroup(gi.GroupId);
                        //groups.Add(gi);
                    }
                }
            }

            return groups;

        }
    }
}
