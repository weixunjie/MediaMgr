using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using MediaMgrSystem.DataModels;
using System.Threading;
using MediaMgrSystem.BusinessLayerLogic;
namespace MediaMgrSystem
{
    public partial class DeviceList : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<GroupInfo> GetAllGroupsIncludeDefaultGroup()
        {
            List<GroupInfo> gis = null;

            if (GlobalUtils.CheckIfVideoFunction())
            {

                gis = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(BusinessType.VIDEOONLINE);

            }
            else
            {
                gis = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(BusinessType.AUDITBROADCAST);

            }


            GroupInfo groupDefault = new GroupInfo();

            groupDefault.GroupId = "-1";
            groupDefault.GroupName = "默认分组";

            if (GlobalUtils.CheckIfVideoFunction())
            {
                groupDefault.Devices = GlobalUtils.DeviceBLLInstance.GetAllDevicesByGroupWithFilter("-1", BusinessType.VIDEOONLINE);

            }
            else
            {
                groupDefault.Devices = GlobalUtils.DeviceBLLInstance.GetAllDevicesByGroupWithFilter("-1", BusinessType.AUDITBROADCAST);

            }



            gis.Insert(0, groupDefault);

            return gis;

        }


        public bool CheckIfPlaying()
        {
            return GlobalUtils.CheckIfPlaying();

        }

        public bool CheckIsSupperUser()
        {
            if (HttpContext.Current != null &&
                HttpContext.Current.Session["IsSuperUser"] != null
                && HttpContext.Current.Session["IsSuperUser"].ToString() == "1")
            {
                return true;
            }

            return false;


        }

   



        public string GetGroupImageUrl()
        {
            string srcName = "ic_image_group.png";
            if ( GlobalUtils.CheckIfVideoFunction()) { srcName = "ic_image_group_video.png"; }


            return srcName;
        }
        public string GetImageUrl(string ipAddress)
        {

            string srcName = "ic_image_device";

            if (!CheckDeviceIsOnline(ipAddress))
            {
                srcName = "ic_image_device_offline";
            }



            if (GlobalUtils.CheckIfVideoFunction())
            {
                srcName = srcName + "_video";
            }

            return srcName;

        }
        public bool CheckDeviceIsOnline(string ipAddress)
        {
            return GlobalUtils.GetConnectionIdsByIdentify(new List<string> { ipAddress }, SingalRClientConnectionType.ANDROID).Count > 0;

        }

        public List<ChannelInfo> GetAllChannels()
        {
            return GlobalUtils.ChannelBLLInstance.GetAllChannels();

        }

        public List<EncoderInfo> GetAllEncoders()
        {
            return GlobalUtils.EncoderBLLInstance.GetAllEncoders();

        }


    }
}