
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem
{
    public class INIFile
    {
        public string path;
        public INIFile(string INIPath)
        {
            path = INIPath;
        }
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
         string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
         string key, string def, StringBuilder retVal, int size, string filePath);
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //  string output = FFMPEG.Execute("c:\ffmpeg.exe");


            //FFMPEG.FFMPEGExecutableFilePath = @"c:\FFMPEG.exe";

            //VideoFile videoFile = new VideoFile(@"c:\hamster.flv");

            //INIFile ab = new INIFile(@"F:\test");
            //string iniFile = @"F:\test\test.ini";
            //if (!File.Exists(iniFile))
            //{
            //    using (FileStream fs = File.Create(iniFile))
            //    {
            //        fs.Close();
            //    }
            //}
            //string[] args = new string[10];
            //INIFile myINI = new INIFile(iniFile);
            //for (int i = 0; i < args.Length; i++)
            //{
            //    args[i] = Convert.ToString(i + i * i * i);
            //    myINI.IniWriteValue("WebDir", "arg" + i.ToString(), args[i]);
            //}
        }

        public string getDataTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");

        }

       [WebMethod]
        public static string RangerUserControl(string controlName)
        {
            StringBuilder build = new StringBuilder();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(new StringWriter(build));
            UserControl uc = new UserControl();
            Control ctrl = uc.LoadControl(controlName + ".ascx");
            htmlWriter.Flush();
            string result;
            try
            {
                ctrl.RenderControl(htmlWriter);
            }
            catch
            {
            }
            finally
            {
                htmlWriter.Flush();
                result = build.ToString();
            }
            return result;
        }

        [WebMethod]
        public static string CheckTimeOutForOperation(string GuidId)
        {
            System.Threading.Thread.Sleep(5000);


            JsonString js = new JsonString();
            js.StrValue = GuidId;

            List<JsonString> strs = new List<JsonString>();
            strs.Add(js);
            return Newtonsoft.Json.JsonConvert.SerializeObject(strs);


        }

        [WebMethod]
        public static string GetScheduleByChannelId(string cid)
        {
         //   Thread.Sleep(2000);
            return "1";

        }

        [WebMethod]
        public static string SaveSchedule(string cid ,string sid)
        {
            //   Thread.Sleep(2000);
            return "1";

        }
      
      
        

    }
}