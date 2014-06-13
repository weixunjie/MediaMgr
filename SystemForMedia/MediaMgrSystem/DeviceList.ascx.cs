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

        private GroupBLL groupBLL = new GroupBLL(GlobalUtils.DbUtilsInstance);

        private DeviceBLL deviceBLL = new DeviceBLL(GlobalUtils.DbUtilsInstance);

        private ChannelBLL channelBLL = new ChannelBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<GroupInfo> GetAllGroupsIncludeDefaultGroup()
        {
            List<GroupInfo> gis = groupBLL.GetAllGroups();

            GroupInfo groupDefault = new GroupInfo();

            groupDefault.GroupId = "-1";
            groupDefault.GroupName = "默认分组";
            groupDefault.Devices = deviceBLL.GetAllDevicesByGroup("-1");

            gis.Insert(0, groupDefault);

            return gis;

        }


        public bool CheckDeviceIsOnline(string ipAddress)
        {
            return GlobalUtils.GetConnectionIdsByIdentify(new List<string> { ipAddress }).Count > 0;

        }

        public List<ChannelInfo> GetAllChannels()
        {
            return channelBLL.GetAllChannels();

        }



    }
}