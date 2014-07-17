
using MediaMgrSystem.BusinessLayerLogic;
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
    public partial class RemoteControlMain : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            //if (Session["UserId"] == null)
            //{
            //    Response.Redirect("~/Login.aspx");
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
            Control ctrl = uc.LoadControl(@"RemoteControlSys\"+controlName + ".ascx");
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
        public static void SaveDeviceGroup(string deivceId, string groupId)
        {

            if (!string.IsNullOrWhiteSpace(deivceId))
            {
                deivceId = deivceId.TrimEnd(',');
                string[] strs = deivceId.Split(',');
                if (strs != null && strs.Length > 0)
                {
                    foreach (var str in strs)
                    {
                        GlobalUtils.DeviceBLLInstance.UpdateDeviceGroup(str, groupId);
                        //deviceBLL
                    }

                }
            }
            //System.Threading.Thread.Sleep(5000);


            //JsonString js = new JsonString();
            //js.StrValue = GuidId;

            //List<JsonString> strs = new List<JsonString>();
            //strs.Add(js);
            //return Newtonsoft.Json.JsonConvert.SerializeObject(strs);


        }

        [WebMethod]
        public static string GetScheduleByChannelId(string cid)
        {
            ChannelInfo ci = GlobalUtils.ChannelBLLInstance.GetChannelById(cid);
            //   Thread.Sleep(2000);
            return ci.ScheduelId;

        }

        [WebMethod]
        public static void SaveSchedule(string cid, string sid)
        {
            GlobalUtils.ChannelBLLInstance.UpdateChannelSchedule(cid, sid);

        }
        [WebMethod]
        public static void SaveGroupChannelAndEncoder(string cid, string gid,string eid)
        {
            GlobalUtils.GroupBLLInstance.UpdateGroupChannelAndEncoder(gid, cid,eid);
        }

        [WebMethod]
        public static void SaveGroupChannel(string cid, string gid)
        {
            GlobalUtils.GroupBLLInstance.UpdateGroupChannel(gid, cid);
        }

        [WebMethod]
        public static void SaveGroupEncoder(string cid, string gid)
        {
            GlobalUtils.GroupBLLInstance.UpdateGroupEncoder(gid, cid);
        }
          
        

        [WebMethod]
        public static string GetChannelByGroupId(string gid)
        {
            //   Thread.Sleep(2000);
            // return "3";

            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetGroupById(gid);

            if (gis != null && gis.Count > 0)
            {

                return gis[0].ChannelId;

            }

            return string.Empty;


        }


        [WebMethod]
        public static string GetEncoderByGroupId(string gid)
        {

            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetGroupById(gid);

            if (gis != null && gis.Count > 0)
            {
                return gis[0].EncoderId;
            }

            return string.Empty;


        }





    }
}