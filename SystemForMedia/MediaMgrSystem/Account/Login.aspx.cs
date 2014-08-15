using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using MediaMgrSystem.Models;

namespace MediaMgrSystem.Account
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected void LogIn(object sender, EventArgs e)
        {
            if (UserName.Text == "admin")
            {
                Session["UserName"] = UserName;
                Response.Redirect("~/BroadcastMain.aspx");
            }
        }
    }
}