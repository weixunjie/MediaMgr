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
    public partial class RemoteDeviceList : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<GroupInfo> GetAllGroupsIncludeDefaultGroup()
        {
            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetAllGroups();

            GroupInfo groupDefault = new GroupInfo();

            groupDefault.GroupId = "-1";
            groupDefault.GroupName = "默认分组";
            groupDefault.Devices = GlobalUtils.DeviceBLLInstance.GetAllDevicesByGroup("-1");

            gis.Insert(0, groupDefault);

            return gis;

        }


        public bool CheckDeviceIsOnline(string ipAddress)
        {
            return GlobalUtils.GetConnectionIdsByIdentify(new List<string> { ipAddress }).Count > 0;

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