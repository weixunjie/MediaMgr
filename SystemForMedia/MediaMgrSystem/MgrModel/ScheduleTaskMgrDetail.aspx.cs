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
    public partial class ScheduleTaskMgrDetail : System.Web.UI.Page
    {
        private ScheduleBLL scheduleBLL = new ScheduleBLL(GlobalUtils.DbUtilsInstance);
        private ProgramBLL programBLL = new ProgramBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                lbMessage.Visible = false;
                List<ProgramInfo> allProgram = programBLL.GetAllProgram();


                this.ddProgram.DataSource = allProgram;

                ddProgram.DataValueField = "ProgramId";

                ddProgram.DataTextField = "ProgramName";

                ddProgram.DataBind();


                ddPriority.Items.Clear();

                for (int i = 1; i <= 10; i++)
                {
                    ddPriority.Items.Add(new ListItem() { Text = i.ToString(), Value = i.ToString() });
                }



                if (Request["sid"] != null)
                {

                    TbHiddenIdSchedule.Text = Request["sid"].ToString();
                }

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();


                    TbHiddenId.Text = id;

                    ScheduleTaskInfo si = scheduleBLL.GetAllScheduleTaskById(id);

                    this.TbName.Text = si.ScheduleTaskName;

                    this.tbStartTime.Value = si.ScheduleTaskStartTime;


                    tbEndTime.Value = si.ScheduleTaskEndTime;


                    this.lbSelectedDate.Items.Clear();

                    if (si.ScheduleTaskSpecialDays != null && si.ScheduleTaskSpecialDays.Count > 0)
                    {

                        foreach (var str in si.ScheduleTaskSpecialDays)
                        {
                            lbSelectedDate.Items.Add(new ListItem() { Text = str, Value = str });
                        }

                    }

                    foreach (ListItem item in CbWeek.Items)
                    {
                        item.Selected = false;
                    }


                    if (si.ScheduleTaskWeeks != null && si.ScheduleTaskWeeks.Count > 0)
                    {

                        foreach (var str in si.ScheduleTaskWeeks)
                        {
                            foreach (ListItem item in CbWeek.Items)
                            {
                                if (item.Value == str)
                                {
                                    item.Selected = true;
                                }
                            }
                            // lbSelectedDate.Items.Add(new ListItem() { Text = str, Value = str });
                        }

                    }


                    var found = ddProgram.Items.FindByValue(si.ScheduleTaskProgarmId);

                    if (found != null)
                    {
                        found.Selected = true;
                    }

                    found = this.ddPriority.Items.FindByValue(si.ScheduleTaskPriority);

                    if (found != null)
                    {
                        found.Selected = true;
                    }
                }
            }
        }

        protected void Unnamed7_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleMgrDetail.aspx?id=" + TbHiddenIdSchedule.Text);
        }

        protected void Unnamed6_Click(object sender, EventArgs e)
        {

            lbMessage.Visible = false;
            DateTime dtStart;
            DateTime dtEnd;

            if (DateTime.TryParse(tbStartTime.Value, out dtStart) &&
                DateTime.TryParse(tbEndTime.Value, out dtEnd))
            {
                if (dtEnd <= dtStart)
                {

                    lbMessage.Text = "结束时间必须大于开始时间";
                    lbMessage.Visible = true;
                    return;

                }
            }

            ScheduleTaskInfo si = new ScheduleTaskInfo();


            si.ScheduleId = TbHiddenIdSchedule.Text;
            si.ScheduleTaskStartTime = tbStartTime.Value;
            si.ScheduleTaskName = TbName.Text;
            si.ScheduleTaskPriority = ddPriority.SelectedValue;
            si.ScheduleTaskEndTime = tbEndTime.Value;

            si.ScheduleTaskProgarmId = ddProgram.SelectedValue;

            si.ScheduleTaskSpecialDays = new List<string>();


            si.StrDays = "";
            si.StrWeeks = "";

            foreach (ListItem lv in lbSelectedDate.Items)
            {
                si.ScheduleTaskSpecialDays.Add(lv.Value);
                si.StrDays += lv.Value + ",";
            }



            si.ScheduleTaskWeeks = new List<string>();
            foreach (ListItem lv in CbWeek.Items)
            {
                if (lv.Selected)
                {
                    si.ScheduleTaskWeeks.Add(lv.Value);
                    si.StrWeeks += lv.Value + ",";
                }
            }

            if (!string.IsNullOrWhiteSpace(si.StrWeeks))
            {
                si.StrWeeks = si.StrWeeks.TrimEnd(',');
            }

            if (!string.IsNullOrWhiteSpace(si.StrDays))
            {
                si.StrDays = si.StrDays.TrimEnd(',');
            }


            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                si.ScheduleTaskId = TbHiddenId.Text;

                scheduleBLL.UpdateScheduleTask(si);
            }
            else
            {
                scheduleBLL.AddSchdeulTask(si);
            }

            Response.Redirect("~/MgrModel/ScheduleMgrDetail.aspx?id=" + TbHiddenIdSchedule.Text);
        }

        protected void btnDelSelected_Click(object sender, EventArgs e)
        {
            if (lbSelectedDate.SelectedItem != null)
            {
                lbSelectedDate.Items.Remove(lbSelectedDate.SelectedItem);
            }
        }

        protected void btnAddDate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSelectDate.Value))
            {
                lbSelectedDate.Items.Add(new ListItem() { Text = tbSelectDate.Value, Value = tbSelectDate.Value });
            }

        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddProgram.SelectedItem != null)
            {
                List<ProgramInfo> pis = programBLL.GetProgramById(ddProgram.SelectedValue,true);

                if (pis != null && pis.Count > 0)
                {

                    if (pis[0].MappingFiles != null && pis[0].MappingFiles.Count > 0)
                    {
                                               
                        string fileUrl = ResolveUrl("~/FileSource/" + pis[0].MappingFiles[0].FileName);

                        string playMap3Page = ResolveUrl("~/MgrModel/PreviewMP3.aspx?FileUrl="+fileUrl);

                        Response.Write("<script language='javascript'>window.open('" + playMap3Page + "','','resizable=1,scrollbars=0');</script>");
                        //  Response.Write("~/MgrModel/ScheduleMgrDetail.aspx?id=" + TbHiddenIdSchedule.Text);
                    }

                }
            }
        }
    }

}