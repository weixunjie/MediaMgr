using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem
{
    public partial class LogOut : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["UserId"] = null;
            Session["UserName"] = null;

            Session["IsLoginPageNow"] = true;

            Response.Redirect("~/Login.aspx");

        }


    }
}