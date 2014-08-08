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
    public partial class ChannelMgrList : System.Web.UI.Page
    {
        private UserBLL channelBLL = new UserBLL(GlobalUtils.DbUtilsInstance);

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
            Response.Redirect("~/MgrModel/ChannelMgrDetail.aspx");
        }


        private void BindListData()
        {
            List<ChannelInfo> datas = GlobalUtils.ChannelBLLInstance.GetAllChannels();

            dvList.DataSource = datas;
            dvList.DataBind();

        }

        protected void dvGroupList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                Response.Redirect("~/MgrModel/ChannelMgrDetail.aspx?id=" + e.CommandArgument.ToString());

            }
            else if (e.CommandName == "Del")
            {
                GlobalUtils.ChannelBLLInstance.RemoveChannel(e.CommandArgument.ToString());
                BindListData();
            }
        }

        protected void dvList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dvList.PageIndex = e.NewPageIndex;
            BindListData();

        }
    }
}