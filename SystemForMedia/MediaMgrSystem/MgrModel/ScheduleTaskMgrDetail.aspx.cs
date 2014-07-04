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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!Page.IsPostBack)
            {


                lbMessage.Visible = false;
                List<ProgramInfo> allProgram = GlobalUtils.ProgramBLLInstance.GetAllProgram();



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

                    ScheduleTaskInfo si = GlobalUtils.ScheduleBLLInstance.GetAllScheduleTaskById(id);

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

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ScheduleMgrDetail.aspx?id=" + TbHiddenIdSchedule.Text);
        }

        protected void Add_Click(object sender, EventArgs e)
        {

            lbMessage.Visible = false;
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;


            if (string.IsNullOrWhiteSpace(tbStartTime.Value) || string.IsNullOrWhiteSpace(tbEndTime.Value))
            {
                return;
            }

            bool isStartTimeOk = false;


            if (tbStartTime.Value.Length == 5 && tbStartTime.Value.IndexOf(":") == 2)
            {
                if (DateTime.TryParse(tbStartTime.Value, out dtStart))
                {

                    isStartTimeOk = true;
                }
            }

            if (!isStartTimeOk)
            {
                lbMessage.Text = "开始时间格式不正确";
                lbMessage.Visible = true;
                return;
            }


            bool isEndTimeOk = false;
            if (tbEndTime.Value.Length == 5 && tbEndTime.Value.IndexOf(":") == 2)
            {
                if (DateTime.TryParse(tbEndTime.Value, out dtEnd))
                {

                    isEndTimeOk = true;
                }
            }

            if (!isEndTimeOk)
            {
                lbMessage.Text = "结束时间格式不正确";
                lbMessage.Visible = true;
                return;
            }
            
            if (dtEnd <= dtStart)
            {
                lbMessage.Text = "结束时间必须大于开始时间";
                lbMessage.Visible = true;
                return;

            }

            ScheduleTaskInfo si = new ScheduleTaskInfo();

           

            si.ScheduleTaskSpecialDays = new List<string>();

            si.ScheduleTaskspecialDaysToWeeks = new List<string>();

            string sqlCheckWeeks = string.Empty;

            string sqlSpecDaysWeekInSchedeulWeek = string.Empty;

            string sqlSpecDaysWeekInSpecialDaysWeek = string.Empty;


            string sqlWeekInSpecDaysWeek = string.Empty;

            string sqlWeekInScheduleWeek = string.Empty;


            si.StrDays = "";
            si.StrWeeks = "";
            si.StrSpecialDaysToWeeks = "";

            foreach (ListItem lv in lbSelectedDate.Items)
            {
                si.ScheduleTaskSpecialDays.Add(lv.Value);
                si.StrDays += lv.Value + ",";


                string strWeek = DateTime.Parse(lv.Value).DayOfWeek.ToString();
                string weekIndex = string.Empty;
                switch (strWeek)
                {
                    case "Monday":
                        weekIndex = "1";
                        break;
                    case "Tuesday":
                        weekIndex = "2";
                        break;
                    case "Wednesday":
                        weekIndex = "3";
                        break;
                    case "Thursday":
                        weekIndex = "4";
                        break;
                    case "Friday":
                        weekIndex = "5";
                        break;
                    case "Saturday":
                        weekIndex = "6";
                        break;
                    case "Sunday":
                        weekIndex = "7";
                        break;

                }




                si.StrSpecialDaysToWeeks += weekIndex + ",";
                si.ScheduleTaskspecialDaysToWeeks.Add(weekIndex);
                sqlSpecDaysWeekInSpecialDaysWeek = sqlSpecDaysWeekInSpecialDaysWeek + " SCHEDULETASKSPECIALDAYSTOWEEKS LIKE '%" + weekIndex + "%' OR";

                sqlSpecDaysWeekInSchedeulWeek = sqlSpecDaysWeekInSchedeulWeek + " SCHEDULETASKWEEKS LIKE '%" + weekIndex + "%' OR";
            }

            si.ScheduleTaskWeeks = new List<string>();
            foreach (ListItem lv in CbWeek.Items)
            {
                if (lv.Selected)
                {
                    si.ScheduleTaskWeeks.Add(lv.Value);
                    si.StrWeeks += lv.Value + ",";

                    sqlWeekInSpecDaysWeek = sqlWeekInSpecDaysWeek + " SCHEDULETASKSPECIALDAYSTOWEEKS LIKE '%" + lv.Value + "%'  OR";

                    sqlWeekInScheduleWeek = sqlWeekInScheduleWeek + " SCHEDULETASKWEEKS LIKE '%" + lv.Value + "%'  OR";
                }
            }

            if (!string.IsNullOrWhiteSpace(sqlSpecDaysWeekInSpecialDaysWeek))
            {
                if (!string.IsNullOrWhiteSpace(sqlWeekInScheduleWeek))
                {
                    sqlCheckWeeks = "AND " + "(" + sqlSpecDaysWeekInSpecialDaysWeek +
                        sqlSpecDaysWeekInSchedeulWeek +
                        sqlWeekInSpecDaysWeek +
                        sqlWeekInScheduleWeek.TrimEnd(new char[] { 'O', 'R' }) + ")";
                }
                else
                {
                    sqlCheckWeeks = "AND " + "(" + sqlSpecDaysWeekInSpecialDaysWeek + sqlSpecDaysWeekInSchedeulWeek.TrimEnd(new char[] { 'O', 'R' }) + ")";

                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(sqlWeekInScheduleWeek))
                {
                    sqlCheckWeeks = "AND " + "(" +
                        sqlWeekInSpecDaysWeek +
                        sqlWeekInScheduleWeek.TrimEnd(new char[] { 'O', 'R' }) + ")";
                }



            }

            if (!string.IsNullOrWhiteSpace(si.StrSpecialDaysToWeeks))
            {
                si.StrSpecialDaysToWeeks = si.StrSpecialDaysToWeeks.TrimEnd(',');
            }

            if (!string.IsNullOrWhiteSpace(si.StrWeeks))
            {
                si.StrWeeks = si.StrWeeks.TrimEnd(',');
            }

            if (!string.IsNullOrWhiteSpace(si.StrDays))
            {
                si.StrDays = si.StrDays.TrimEnd(',');
            }



            if (GlobalUtils.ScheduleBLLInstance.CheckScheduleTaskTimeIsOverLap(TbHiddenIdSchedule.Text, tbStartTime.Value, tbEndTime.Value, TbHiddenId.Text, sqlCheckWeeks))
            {
                lbMessage.Text = "时间与该计划的其他任务时间段有冲突";
                lbMessage.Visible = true;
                return;
            }



            si.ScheduleId = TbHiddenIdSchedule.Text;
            si.ScheduleTaskStartTime = tbStartTime.Value;
            si.ScheduleTaskName = TbName.Text;
            si.ScheduleTaskPriority = ddPriority.SelectedValue;
            si.ScheduleTaskEndTime = tbEndTime.Value;

            si.ScheduleTaskProgarmId = ddProgram.SelectedValue;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                si.ScheduleTaskId = TbHiddenId.Text;

                GlobalUtils.ScheduleBLLInstance.UpdateScheduleTask(si);
            }
            else
            {
                GlobalUtils.ScheduleBLLInstance.AddSchdeulTask(si);
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
                List<ProgramInfo> pis = GlobalUtils.ProgramBLLInstance.GetProgramById(ddProgram.SelectedValue, true);

                if (pis != null && pis.Count > 0)
                {

                    if (pis[0].MappingFiles != null && pis[0].MappingFiles.Count > 0)
                    {

                        string fileUrl = ResolveUrl("~/FileSource/" + pis[0].MappingFiles[0].FileName);

                        //string playMap3Page = ResolveUrl("~/MgrModel/PreviewMP3.aspx?FileUrl=" + fileUrl);

                        string baseUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.IndexOf(Request.RawUrl));
                        string playMap3Page = baseUrl + fileUrl;

                        ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "openFileScript", "window.open('" + playMap3Page + "','','resizable=1,scrollbars=0');", true);

                        //    ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "openFileScript", "window.open('" + playMap3Page + "','','resizable=1,scrollbars=0');", true);

                        //  Response.Write("~/MgrModel/ScheduleMgrDetail.aspx?id=" + TbHiddenIdSchedule.Text);
                    }

                }
            }
        }
    }

}