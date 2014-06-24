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
            List<ScheduleInfo> dis = GlobalUtils.ScheduleBLLInstance.GetAllSchedules();

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
                GlobalUtils.ScheduleBLLInstance.RemoveSchedule(e.CommandArgument.ToString());
                BindListData();
            }
        }
       
    }
}