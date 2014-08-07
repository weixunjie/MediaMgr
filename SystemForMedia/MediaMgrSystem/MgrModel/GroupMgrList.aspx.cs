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
    public partial class GroupMgrList : System.Web.UI.Page
    {
        private GroupBLL groupBLL = new GroupBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
  
            if (!Page.IsPostBack)
            {
                BindListData();
            }
        }



        protected void Add_Click(object sender, EventArgs e)
        {

            // ListBox1.Items.RemoveAt(1);
            Response.Redirect("~/MgrModel/GroupMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<GroupInfo> groups = groupBLL.GetAllGroups();

            dvGroupList.DataSource = groups;
            dvGroupList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/GroupMgrDetail.aspx?gid=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                if (GlobalUtils.CheckIfPlaying())
                {
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForGroupList", "alert('分组正在使用，不能删除');", true);
                    return;
                }

                groupBLL.RemoveGroup(e.CommandArgument.ToString());
                BindListData();
            }
        }
    }
}