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
    public partial class ScheduleMgrList : System.Web.UI.Page
    {

        private ScheduleBLL scheduleBLL = new ScheduleBLL(GlobalUtils.DbUtilsInstance);
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
            Response.Redirect("~/MgrModel/ScheduleMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<ScheduleInfo> dis = scheduleBLL.GetAllSchedules();

            dvList.DataSource = dis;
            dvList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/ScheduleMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                scheduleBLL.RemoveSchedule(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {

                    List<ProgramInfo> result = this.programBLL.GetProgramById(e.Row.Cells[2].Text);

                    if (result != null && result.Count > 0)
                    {
                        e.Row.Cells[2].Text = result[0].ProgramName;
                    }
                }
                catch { }
            }
        }
    }
}