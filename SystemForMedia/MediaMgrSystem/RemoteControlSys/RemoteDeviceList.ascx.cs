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

        public List<GroupInfo> GetAllGroups()
        {
            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(BusinessType.REMOVECONTROL);                   

            return gis;

        }


        public bool CheckDeviceIsOnline(string ipAddress)
        {
            List<string> ipReallySent = new List<string>();
            return GlobalUtils.GetConnectionIdsByIdentify(new List<string> { ipAddress }, SingalRClientConnectionType.REMOTECONTORLDEVICE, out ipReallySent).Count > 0;

        }


    }
}