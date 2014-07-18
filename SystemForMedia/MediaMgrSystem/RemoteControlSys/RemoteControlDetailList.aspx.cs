
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
    public partial class RemoteControlDetailList : Page
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

        public List<GroupInfo> GetAllGroups()
        {
            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetAllGroups();

            return gis;

        }
    }
}