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
    public partial class EncoderVideoMgrList : System.Web.UI.Page
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

    
            Response.Redirect("~/MgrModel/EncoderVideoMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<VideoEncoderInfo> datas = GlobalUtils.VideoEncoderBLLInstance.GetAllEncoders();

            dvList.DataSource = datas;
            dvList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/EncoderVideoMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                GlobalUtils.VideoEncoderBLLInstance.RemoveEncoder(e.CommandArgument.ToString());
                BindListData();
            }
        }
    }
}