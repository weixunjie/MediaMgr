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
    public partial class DeviceMgrList : System.Web.UI.Page
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
        
            Response.Redirect("~/MgrModel/DeviceMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<DeviceInfo> dis = GlobalUtils.DeviceBLLInstance.GetAllDevices();
            List<DeviceInfo> disOnline = new List<DeviceInfo>();
            List<DeviceInfo> disOfflineline = new List<DeviceInfo>();

            List<DeviceInfo> all = new List<DeviceInfo>();
            if (dis != null)
            {
                foreach (var di in dis)
                {
                    List<string> ipReallySent = new List<string>();
                    if (GlobalUtils.GetConnectionIdsByIdentify(new List<string> { di.DeviceIpAddress }, SingalRClientConnectionType.ANDROID, out ipReallySent).Count > 0)
                    {
                        disOnline.Add(di);

                    }
                    else
                    {
                        disOfflineline.Add(di);
                    }
                }

            }

            all = disOnline;
            all.AddRange(disOfflineline);

            dvList.DataSource = all;
            dvList.DataBind();

        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dvList.Rows.Count; i++)
            {
                ((CheckBox)this.dvList.Rows[i].FindControl("chkItem")).Checked =
                    ((CheckBox)this.dvList.HeaderRow.FindControl("chkAll")).Checked;
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            bool isDelSomething = false;
            for (int i = 0; i < this.dvList.Rows.Count; i++)
            {
                if (((CheckBox)this.dvList.Rows[i].FindControl("chkItem")).Checked)
                {
                    Label idLb = (Label)this.dvList.Rows[i].FindControl("lbId");

                    if (idLb != null && !string.IsNullOrEmpty(idLb.Text))
                    {
                        if (GlobalUtils.CheckIfPlaying())
                        {

                            ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForSheduleDetail", "alert('设备正在使用，不能删除');", true);

                            return;
                        }

                        GlobalUtils.DeviceBLLInstance.RemoveDevice(idLb.Text);
                        isDelSomething = true;
                    }
                }

            }

            if (isDelSomething)
            {
                BindListData();
            }
        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/DeviceMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {

                if (GlobalUtils.CheckIfPlaying())
                {
                    ScriptManager.RegisterStartupScript(this.UpdatePanel1, this.GetType(), "alertForDeviceList", "alert('设备正在使用，不能删除');", true);
                    return;
                }

                GlobalUtils.DeviceBLLInstance.RemoveDevice(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TableCell groupCell = e.Row.Cells[3];
                List<GroupInfo> result= GlobalUtils.GroupBLLInstance.GetGroupById(groupCell.Text);
                groupCell.Text = "默认分组";
                if (result != null && result.Count > 0)
                {
                    groupCell.Text = result[0].GroupName;
                }
            }
        }

        protected void dvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dvList.PageIndex = e.NewPageIndex;
            BindListData();

        }
    }
}