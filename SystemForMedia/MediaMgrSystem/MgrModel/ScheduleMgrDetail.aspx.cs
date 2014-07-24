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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                }


                List<ProgramInfo> allProgram = GlobalUtils.ProgramBLLInstance.GetAllProgram();




                divTask.Visible = false;
                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    ScheduleInfo si = GlobalUtils.ScheduleBLLInstance.GetAllScheduleById(id)[0];

                    this.TbName.Text = si.ScheduleName;

                    this.BindTaskListData();


                    divTask.Visible = true;

                }



            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            ScheduleInfo si = new ScheduleInfo();

            si.ScheduleName = TbName.Text;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                si.ScheduleId = TbHiddenId.Text;

                GlobalUtils.ScheduleBLLInstance.UpdateSchedule(si);
            }
            else
            {
                GlobalUtils.ScheduleBLLInstance.AddSchedule(si);
            }

            Response.Redirect("~/MgrModel/ScheduleMgrList.aspx");
        }

        private void BindTaskListData()
        {
            List<ScheduleTaskInfo> dis = GlobalUtils.ScheduleBLLInstance.GetAllScheduleTaksByScheduleId(TbHiddenId.Text);

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
                if (GlobalUtils.ScheduleBLLInstance.CheckScheduleTaskIsRunning(e.CommandArgument.ToString()))
                {

                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForSheduleDetail", "alert('计划任务运行中，不能删除');", true);
              
                    return;
                }
                GlobalUtils.ScheduleBLLInstance.RemoveSchdeulTask(e.CommandArgument.ToString());
                BindTaskListData();

            }
        }

        protected void AddTask_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleTaskMgrDetail.aspx?sid=" + TbHiddenId.Text);
        }

        protected void dvTaskList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            dvTaskList.PageIndex = e.NewPageIndex;
            BindTaskListData();
        }

        protected void dvTaskList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                try
                {
                    List<ProgramInfo> result = GlobalUtils.ProgramBLLInstance.GetProgramById(e.Row.Cells[6].Text);

                    if (result != null && result.Count > 0)
                    {
                        e.Row.Cells[6].Text = result[0].ProgramName;
                    }
                }
                catch { }

                string strWeeks = e.Row.Cells[7].Text;

                strWeeks = strWeeks.Replace("1", "一");

                strWeeks = strWeeks.Replace("2", "二");
                strWeeks = strWeeks.Replace("3", "三");

                strWeeks = strWeeks.Replace("4", "四");

                strWeeks = strWeeks.Replace("5", "五");

                strWeeks = strWeeks.Replace("6", "六");

                strWeeks = strWeeks.Replace("7", "日");

                e.Row.Cells[7].Text = strWeeks;



            }

        }
    }
}