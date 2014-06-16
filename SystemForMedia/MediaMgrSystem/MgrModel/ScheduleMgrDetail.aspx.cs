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




                this.ddProgram.DataSource = allProgram;

                ddProgram.DataValueField = "ProgramId";

                ddProgram.DataTextField = "ProgramName";

                ddProgram.DataBind();


                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    ScheduleInfo si = scheduleBLL.GetAllScheduleById(id)[0];

                    this.TbName.Text = si.ScheduleName;

                    this.TBTime.Text = si.ScheduleTime;



                    


                    var found = ddProgram.Items.FindByValue(si.ProgrameId);

                    if (found != null)
                    {
                        found.Selected = true;
                    }


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

            si.ScheduleTime = TBTime.Text;



            si.ProgrameId = this.ddProgram.SelectedItem.Value;




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
    }
}