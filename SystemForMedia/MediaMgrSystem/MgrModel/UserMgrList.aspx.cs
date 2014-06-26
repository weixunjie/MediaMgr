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
            List<UserInfo> data = GlobalUtils.UserBLLInstance.GetAllUses();

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
                GlobalUtils.ProgramBLLInstance.RemoveProgram(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                string userLevel = e.Row.Cells[3].Text;

                e.Row.Cells[3].Text = userLevel == "1" ? "超级用户" : "普通用户";
                string isActive = e.Row.Cells[4].Text;

                e.Row.Cells[4].Text = isActive == "1" ? "是" : "否";
            }

        }
    }
}