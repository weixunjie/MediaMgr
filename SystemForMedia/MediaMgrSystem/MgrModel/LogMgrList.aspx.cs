using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem.MgrModel
{
    public partial class LogMgrList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {
                if (Session["IsSuperUser"] != null)
                {
                    if (Session["IsSuperUser"].ToString() != "1")
                    {
                        Response.Redirect("~/Login.aspx");
                    }

                }
            }

            if (!Page.IsPostBack)
            {


                tbStartDate.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
                tbEndDate.Value = System.DateTime.Now.ToString("yyyy-MM-dd");
                BindListData();
            }
        }

        protected void Search_Click(object sender, EventArgs e)
        {
            BindListData();

            // ListBox1.Items.RemoveAt(1);
            //Response.Redirect("~/MgrModel/GroupMgrDetail.aspx");
        }


        public void BindListData()
        {
            List<LogInfo> logs = GlobalUtils.LogBLLInstance.GetLogsByCriteria(TBName.Text, tbStartDate.Value, tbEndDate.Value);

            dvList.DataSource = logs;
            dvList.DataBind();

        }

       

        protected void dvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                GlobalUtils.LogBLLInstance.RemoveLog(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
          
            dvList.PageIndex = e.NewPageIndex;
            BindListData();
        }

        protected void btnClearLog_Click(object sender, EventArgs e)
        {
            GlobalUtils.LogBLLInstance.RemoveLogByDayBefore(ddDateBefore.SelectedValue).ToString();


            ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "updateScript", "alert('删除成功！')", true);

         

        
        }




    }
}