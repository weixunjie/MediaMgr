using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem.MgrModel
{
    public partial class UserMgrDetail : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (Session["IsSuperUser"] != null)
                {
                    if (Session["IsSuperUser"].ToString() != "1")
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                }
            }

            if (!Page.IsPostBack)
            {                

                ddUserLevel.SelectedIndex = 0;

                lbMessage.Visible = false;

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    tbHiddenId.Text = id;

                    UserInfo ui = GlobalUtils.UserBLLInstance.GetUserById(id);

                    if (ui.UserId == Session["UserId"].ToString() && Session["UserCode"].ToString().ToUpper() == "ADMIN")
                    {
                        tbUserCode.Enabled = false;
                        chIsAcitve.Enabled = false;
                        ddUserLevel.Enabled = false;
                    }
                    else
                    {
                        tbUserCode.Enabled = true;
                        chIsAcitve.Enabled = true;
                        ddUserLevel.Enabled = true;
                    }


                    this.tbUserCode.Text = ui.UserCode;
                    this.tbName.Text = ui.UserName;
                    tbPassword.Attributes.Add("value", ui.Password);
                    tbConfimedPass.Attributes.Add("value", ui.Password);
                    this.chIsAcitve.Checked = ui.IsActive;
                    this.ddUserLevel.SelectedValue = ui.UserLevel;

                }
                else
                {
                    this.chIsAcitve.Checked = true;
                }

            }
        }


        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/UserMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {

            lbMessage.Visible = false;

            if (!tbPassword.Text.Equals(tbConfimedPass.Text))
            {
                lbMessage.Visible = true;
                lbMessage.Text = "确认密码必须一致";
                return;
            }

            UserInfo ui = new UserInfo();

            ui.IsActive = chIsAcitve.Checked;

            ui.UserCode = tbUserCode.Text;
            ui.UserName = tbName.Text;
            ui.UserLevel = ddUserLevel.SelectedValue;

            ui.Password = tbPassword.Text;


            if (!string.IsNullOrEmpty(tbHiddenId.Text))
            {
                ui.UserId = tbHiddenId.Text;
                GlobalUtils.UserBLLInstance.UpdateUser(ui);
            }
            else
            {
                GlobalUtils.UserBLLInstance.AddUser(ui);

            }

            Response.Redirect("~/MgrModel/UserMgrList.aspx");
        }
    }
}