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
    public partial class ProgramMgrList : System.Web.UI.Page
    {
 
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
            Response.Redirect("~/MgrModel/ProgramMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<ProgramInfo> programes = GlobalUtils.ProgramBLLInstance.GetAllProgram();

            dvGroupList.DataSource = programes;
            dvGroupList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/ProgramMgrDetail.aspx?pid=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {

                if (GlobalUtils.ScheduleBLLInstance.CheckProgrameIsUsing(e.CommandArgument.ToString()))
                {
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForProgameList", "alert('节目被计划任务使用中，不能删除');", true);
                    return;
                }

                GlobalUtils.ProgramBLLInstance.RemoveProgram(e.CommandArgument.ToString());
                BindListData();
            }
        }
    }
}