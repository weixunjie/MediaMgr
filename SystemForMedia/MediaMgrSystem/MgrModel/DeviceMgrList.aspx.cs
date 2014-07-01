﻿using MediaMgrSystem.BusinessLayerLogic;
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

            // ListBox1.Items.RemoveAt(1);
            Response.Redirect("~/MgrModel/DeviceMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<DeviceInfo> dis = GlobalUtils.DeviceBLLInstance.GetAllDevices();

            dvList.DataSource = dis;
            dvList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/DeviceMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                GlobalUtils.DeviceBLLInstance.RemoveDevice(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                List<GroupInfo> result= GlobalUtils.GroupBLLInstance.GetGroupById(e.Row.Cells[2].Text);
                e.Row.Cells[2].Text = "默认分组";
                if (result != null && result.Count > 0)
                {
                    e.Row.Cells[2].Text = result[0].GroupName;
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