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

        public FileAttribute GetFileInfoByFilePath(string filePath)
        {
            String sqlStr = "SELECT * FROM FILEINFO WHERE FILEPATH='" + filePath.Replace("'", "''") + "'";

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

                        fi.FileRelatePath = dt.Rows[i]["FILEPATH"].ToString();
                        fi.BitRate = dt.Rows[i]["BITRATE"].ToString();

                        result = fi;
                    }
                }
            }

            return result;
        }


         List<FileInfo> fileList=new List<FileInfo>();

       void GetAll(DirectoryInfo dir)//搜索文件夹中的文件
        {
        
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                fileList.Add(fi);
            }

            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAll(d);
            }       
        }


        public List<FileAttribute> GetAllDiskFiles(string filePath, string mpegPath)
        {
            List<FileAttribute> result = new List<FileAttribute>();

            DirectoryInfo di = new DirectoryInfo(filePath);

            DirectoryInfo d = new DirectoryInfo(filePath);
            fileList = new List<FileInfo>();
            GetAll(d);



            //  DirectoryInfo[] dirInfo = di.GetDirectories();

            if (fileList != null && fileList.Count > 0)
            {

                foreach (FileInfo nextFile in fileList)  //遍历文件
                {
                    if (nextFile.Extension.ToUpper().EndsWith("META"))
                    {
                        continue;
                    }

                    if (!nextFile.Extension.ToUpper().EndsWith("MP4") && !nextFile.Extension.ToUpper().EndsWith("MP3") && !nextFile.Extension.ToUpper().EndsWith("FLV"))
                    {
                        continue;
                    }

                    string pathName = nextFile.FullName.Replace(filePath, "").TrimStart('\\');

                    FileAttribute fa = GetFileInfoByFilePath(pathName);
       
                    if (fa != null)
                    {
                        result.Add(fa);
                    }
                    else
                    {
                        fa = new FileAttribute();

                        fa.BitRate = GetBitRateByFileName(nextFile.FullName, mpegPath).ToString();
                        fa.FileName = nextFile.Name;
                        fa.FileRelatePath = pathName;
                        AddFileInfo(fa);
                        result.Add(fa);
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
            String sqlStr = "INSERT INTO FILEINFO(FILENAME,BITRATE,FILEPATH) values ('{0}','{1}','{2}')";


            sqlStr = String.Format(sqlStr, fa.FileName.Replace("'", "''"), fa.BitRate,fa.FileRelatePath.Replace("'", "''"));


            return dbUitls.ExecuteNonQuery(sqlStr);

        }

    }
}
