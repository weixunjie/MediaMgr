using System;
using System.Web;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using MediaMgrSystem.Models;

namespace MediaMgrSystem.Account
{
    public partial class LoginOut : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            Session["UserName"] = null;
            Response.Redirect("~/Login.aspx");

        }

        protected void LogIn(object sender, EventArgs e)
        {

        }
    }
}