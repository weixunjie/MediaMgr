using AmagicServer.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace AmagicServer

{   

    public partial class WebPay : System.Web.UI.Page
    {
        private DeviceInfoBLL deviceInfoBLL = new DeviceInfoBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
             if (deviceInfoBLL.CheckDeviceByPhoneSN(Request["snNumber"].ToString()))
             {        
                 Response.Redirect("AlreadyPaied.aspx");
             }

        }
    }
}