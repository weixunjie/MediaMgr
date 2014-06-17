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
    public partial class ScheduleMgrDetail : System.Web.UI.Page
    {
        private ScheduleBLL scheduleBLL = new ScheduleBLL(GlobalUtils.DbUtilsInstance);
        private ProgramBLL programBLL = new ProgramBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                List<ProgramInfo> allProgram = programBLL.GetAllProgram();




                divTask.Visible = false;
                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    ScheduleInfo si = scheduleBLL.GetAllScheduleById(id)[0];

                    this.TbName.Text = si.ScheduleName;




                    divTask.Visible = true;

                }



            }
        }

        protected void Unnamed7_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleMgrList.aspx");
        }

        protected void Unnamed6_Click(object sender, EventArgs e)
        {
            ScheduleInfo si = new ScheduleInfo();

            si.ScheduleName = TbName.Text;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                si.ScheduleId = TbHiddenId.Text;

                scheduleBLL.UpdateSchedule(si);
            }
            else
            {
                scheduleBLL.AddSchedule(si);
            }

            Response.Redirect("~/MgrModel/ScheduleMgrList.aspx");
        }

        private void BindSTaskListData()
        {
            List<ScheduleTaskInfo> dis = scheduleBLL.GetAllScheduleTaksByScheduleId(TbHiddenId.Text);

            this.dvTaskList.DataSource = dis;
            dvTaskList.DataBind();

        }

        protected void dvTaskList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/ScheduleTaskMgrDetail.aspx?id=" + e.CommandArgument.ToString() + "&sid=" + TbHiddenId.Text);

            }
            else if (e.CommandName == "Del")
            {
                scheduleBLL.RemoveSchedule(e.CommandArgument.ToString());
                BindSTaskListData();

            }
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleTaskMgrDetail.aspx?sid=" + TbHiddenId.Text);
        }

        protected void dvTaskList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {

                    List<ProgramInfo> result = this.programBLL.GetProgramById(e.Row.Cells[6].Text);

                    if (result != null && result.Count > 0)
                    {
                        e.Row.Cells[6].Text = result[0].ProgramName;
                    }
                }
                catch { }
            }

        }
    }
}