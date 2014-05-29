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
namespace MediaMgrSystem
{
    public partial class DeviceList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<GroupInfo> GetAllGroups()
        {
            List<GroupInfo> goups = new List<GroupInfo>();

            GroupInfo gi = new GroupInfo { GroupId = "1", GroupName = "moren" };


            gi.Devices = GetDeviceList();

            goups.Add(gi);
            goups.Add(new GroupInfo { GroupId = "1", GroupName = "ddf" });

            return goups;

        }

        public List<DeviceInfo> GetDeviceList()
        {
            string[] str = System.Configuration.ConfigurationManager.AppSettings["IpAddress"].ToString().Split(',');

            List<DeviceInfo> result = new List<DeviceInfo>();
            for (int i = 0; i < str.Length; i++)
            {
                DeviceInfo di = new DeviceInfo();
                di.DeviceId = (i + 1).ToString();
                di.DeviceIpAddress = str[i];
                di.DeviceName = "设备"+i.ToString();
                result.Add(di);
            }

            return result;
        }



    }
}