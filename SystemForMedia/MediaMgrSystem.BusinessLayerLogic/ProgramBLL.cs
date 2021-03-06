﻿using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.BusinessLayerLogic
{
    public class ProgramBLL
    {

        DbUtils dbUitls = null;

        FileInfoBLL fileInfoBLL = null;
        public ProgramBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;
            fileInfoBLL = new FileInfoBLL(dbUitls);

        }

        public List<ProgramInfo> GetProgramById(string pid, bool isGetFileInfo = false)
        {
            String sqlStr = "SELECT * FROM PROGRAMINFO WHERE PROGRAMID=" + pid;

            return GetProgramList(sqlStr, isGetFileInfo);
        }

        public List<ProgramInfo> GetProgramByIds(string[] pids)
        {
            string ids = string.Empty;

            if (pids != null && pids.Length > 0)
            {
                foreach (var str in pids)
                {
                    ids = ids + "'" + str + "',";

                }

                ids = ids.TrimEnd(',');
            }

            if (!string.IsNullOrWhiteSpace(ids))
            {


                String sqlStr = "SELECT * FROM PROGRAMINFO WHERE PROGRAMID in (" + ids + ")";

                return GetProgramList(sqlStr, true);
            }
            else
            {
                return null;
            }
        }



        public List<ProgramInfo> GetAllProgram(bool isGetFileInfo = false)
        {
            String sqlStr = "SELECT * FROM PROGRAMINFO";

            return GetProgramList(sqlStr, isGetFileInfo);
        }

        public List<ProgramInfo> GetAllAuditProgram(bool isGetFileInfo = false)
        {
            String sqlStr = "SELECT *FROM PROGRAMINFO WHERE MAPPINGFILES LIKE '%.MP3%'";

            return GetProgramList(sqlStr, isGetFileInfo);
        }

        public List<ProgramInfo> GetAllVideoProgram(bool isGetFileInfo = false)
        {
            String sqlStr = "SELECT *FROM PROGRAMINFO WHERE MAPPINGFILES NOT LIKE '%.MP3%'";

            return GetProgramList(sqlStr, isGetFileInfo);
        }







        public int RemoveProgram(string pid)
        {
            String sqlStr = "DELETE FROM PROGRAMINFO WHERE PROGRAMID=" + pid;

            return dbUitls.ExecuteNonQuery(sqlStr);

        }

        public int AddPrograme(ProgramInfo pi)
        {
            String sqlStr = "INSERT INTO PROGRAMINFO(PROGRAMNAME,MAPPINGFILES) values ('{0}','{1}')";
            string mapFiles;

            GetMappingValues(pi, out mapFiles);

            sqlStr = String.Format(sqlStr, pi.ProgramName, mapFiles.Replace("'", "''"));

            int result = dbUitls.ExecuteNonQuery(sqlStr);



            return result;

        }

        private void GetMappingValues(ProgramInfo pi, out string mapFiles)
        {
            mapFiles = string.Empty;

            if (pi.MappingFiles != null)
            {
                foreach (var fa in pi.MappingFiles)
                {
                    mapFiles += fa.FileRelatePath + "|";
                }

                mapFiles = mapFiles.TrimEnd('|');

            }
        }


        public int UpdateProgram(ProgramInfo pi)
        {
            String sqlStr = "UPDATE PROGRAMINFO SET PROGRAMNAME='{0}',MAPPINGFILES='{1}'  WHERE PROGRAMID={2}";

            string mapFiles;

            GetMappingValues(pi, out mapFiles);

            sqlStr = String.Format(sqlStr, pi.ProgramName, mapFiles.Replace("'", "''"), pi.ProgramId);

            return dbUitls.ExecuteNonQuery(sqlStr);

        }


        protected List<ProgramInfo> GetProgramList(string sqlStr, bool isGetFileInfo)
        {
            List<ProgramInfo> result = new List<ProgramInfo>();
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


                        if (isGetFileInfo)
                        {
                            string mappingFiles = dt.Rows[i]["MappingFiles"].ToString();


                            if (!string.IsNullOrEmpty(mappingFiles))
                            {
                                string[] tmpMfile = mappingFiles.Split('|');

                                pi.MappingFiles = new List<FileAttribute>();
                                for (int k = 0; k < tmpMfile.Length; k++)
                                {
                                    FileAttribute fa = new FileAttribute();
                                    fa.FileName = tmpMfile[k];

                                    FileAttribute tmp = fileInfoBLL.GetFileInfoByFilePath(tmpMfile[k]);

                                    fa.FileRelatePath = tmp.FileRelatePath;

                                    if (tmp != null)
                                    {
                                        fa.BitRate = tmp.BitRate;
                                    }
                                    else
                                    {
                                        fa.BitRate = "0";
                                    }
                                    pi.MappingFiles.Add(fa);

                                }
                            }
                        }

                        result.Add(pi);


                    }
                }
            }

            return result;

        }
    }
}
