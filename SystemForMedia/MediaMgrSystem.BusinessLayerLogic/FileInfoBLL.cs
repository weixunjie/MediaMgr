using FFMPEGWrapper;
using MediaMgrSystem.DataAccessLayer;
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
    public class FileInfoBLL
    {

        DbUtils dbUitls = null;
        public FileInfoBLL(DbUtils dUtils)
        {
            dbUitls = dUtils;

        }

        public FileAttribute GetFileInfoByFile(string fileName)
        {
            String sqlStr = "SELECT * FROM FILEINFO WHERE FILENAME='" + fileName.Replace("'", "''") + "'";

            FileAttribute result = null;
            DataTable dt = dbUitls.ExecuteDataTable(sqlStr);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        FileAttribute fi = new FileAttribute();
                        fi.FileName = dt.Rows[i]["FILENAME"].ToString();

                        fi.BitRate = dt.Rows[i]["BITRATE"].ToString();

                        result = fi;
                    }
                }
            }

            return result;
        }

        public List<FileAttribute> GetAllDiskFiles(string filePath, string mpegPath)
        {
            List<FileAttribute> result = new List<FileAttribute>();

            DirectoryInfo di = new DirectoryInfo(filePath);

            if (di != null)
            {
                FileInfo[] fiArray = di.GetFiles();
                if (fiArray != null && fiArray.Length > 0)
                {


                    foreach (var fi in fiArray)
                    {
                        if (fi.Extension.ToUpper().EndsWith("META"))
                        {
                            continue;
                        }

                        if (!fi.Extension.ToUpper().EndsWith("MP4") && !fi.Extension.ToUpper().EndsWith("MP3") && !fi.Extension.ToUpper().EndsWith("FLV"))
                        {
                            continue;
                        }


                        FileAttribute fa = GetFileInfoByFile(fi.Name);
                        if (fa != null)
                        {
                            result.Add(fa);
                        }
                        else
                        {
                            fa = new FileAttribute();

                            fa.BitRate = GetBitRateByFileName(fi.FullName, mpegPath).ToString();
                            fa.FileName = fi.Name;
                            AddFileInfo(fa);
                            result.Add(fa);
                        }
                    }


                }
            }

            return result;



            //  string filePath = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connString"].ToString();

        }


        private double GetBitRateByFileName(string fileFullName, string mpegPath)
        {

            double result = 0;

            FFMPEG.FFMPEGExecutableFilePath = mpegPath + @"\ffmpeg.exe";

            VideoFile videoFile = new VideoFile(fileFullName);

            result = videoFile.AudioBitRate;



            return result;

        }

        public string GetVideoFormatByFileName(string fileFullName, string mpegPath)
        {

            string strLongVideoFormat = string.Empty;

            FFMPEG.FFMPEGExecutableFilePath = mpegPath + @"\ffmpeg.exe";

            VideoFile videoFile = new VideoFile(fileFullName);

            strLongVideoFormat = videoFile.VideoFormat;


            if (!string.IsNullOrWhiteSpace(strLongVideoFormat))
            {

                int intCons = strLongVideoFormat.IndexOf("(");

                if (intCons > 0)
                {
                    strLongVideoFormat = strLongVideoFormat.Substring(0, intCons);

                }


                string[] longVideoFormatArrage = strLongVideoFormat.Split(':');

                if (longVideoFormatArrage != null && longVideoFormatArrage.Length >= 2)
                {
                    return longVideoFormatArrage[1].Trim();
                }

            }

            return string.Empty;


        }



        public int AddFileInfo(FileAttribute fa)
        {
            String sqlStr = "INSERT INTO FILEINFO(FILENAME,BITRATE) values ('{0}','{1}')";


            sqlStr = String.Format(sqlStr, fa.FileName.Replace("'", "''"), fa.BitRate);


            return dbUitls.ExecuteNonQuery(sqlStr);

        }

    }
}
