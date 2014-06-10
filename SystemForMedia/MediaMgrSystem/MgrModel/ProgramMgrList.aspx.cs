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
        private ProgramBLL programBLL = new ProgramBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
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
            List<ProgramInfo> programes = programBLL.GetAllProgram();

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
                programBLL.RemoveProgram(e.CommandArgument.ToString());
                BindListData();
            }
        }
    }
}