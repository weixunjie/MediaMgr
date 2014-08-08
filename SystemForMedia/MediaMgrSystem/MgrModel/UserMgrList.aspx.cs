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
    public partial class UserMgrList : System.Web.UI.Page
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
                BindListData();
            }
        }



        protected void Add_Click(object sender, EventArgs e)
        {

            // ListBox1.Items.RemoveAt(1);
            Response.Redirect("~/MgrModel/UserMgrDetail.aspx");
        }


        private void BindListData()
        {

            List<UserInfo> data = null;

            if (Session["UserCode"] != null && Session["UserCode"].ToString().ToUpper() == "ADMIN")
            {
                data = GlobalUtils.UserBLLInstance.GetAllUsesWithAdmin();
            }
            else
            {
                data = GlobalUtils.UserBLLInstance.GetAllUses();
            }

            dvList.DataSource = data;
            dvList.DataBind();

        }


        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/UserMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                string userId = e.CommandArgument.ToString();

                if (userId == Session["UserId"].ToString() && Session["UserCode"].ToString().ToUpper() == "ADMIN")
                {
                    Response.Write("<script>alert('不能删除系统管理员');</script>");
                }
                else
                {
                    GlobalUtils.ProgramBLLInstance.RemoveProgram(userId);
                    BindListData();
                }
            }
        }

        protected void dvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                string userLevel = e.Row.Cells[4].Text;

                e.Row.Cells[4].Text = userLevel == "1" ? "超级用户" : "普通用户";
                string strIsActive = e.Row.Cells[5].Text;
                if (!string.IsNullOrWhiteSpace(strIsActive))
                {
                    e.Row.Cells[5].Text = strIsActive.ToUpper() == "TRUE" ? "是" : "否";
                }
            }

        }

        protected void dvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dvList.PageIndex = e.NewPageIndex;
            BindListData();
        }

       
    }
}