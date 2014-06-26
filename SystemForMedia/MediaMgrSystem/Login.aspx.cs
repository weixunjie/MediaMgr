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
    public partial class Login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["IsLoginPageNow"] = true;
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            this.lbMessage.Visible = false;
            UserInfo ui = GlobalUtils.UserBLLInstance.GetUserByCritiea(tbUserName.Text, tbPassword.Text);

            if (ui != null)
            {
                Session["UserId"] = ui.UserId;
                Session["UserName"] = ui.UserName;
                Session["IsSuperUser"]=ui.UserLevel=="1"?"1":null;
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                lbMessage.Visible = true;
                //Response.Write("<script>alert('登录失败');</script>");

            }




        }


    }
}