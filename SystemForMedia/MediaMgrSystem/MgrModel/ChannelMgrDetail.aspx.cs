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
    public partial class ChannelMgrDetail : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }      

            if (!Page.IsPostBack)
            {

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    ChannelInfo ci = GlobalUtils.ChannelBLLInstance.GetChannelById(id);

                    this.TbName.Text = ci.ChannelName;


                }
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ChannelMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            ChannelInfo ci = new ChannelInfo();


            ci.ChannelName = this.TbName.Text;

            
            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                ci.ChannelId = TbHiddenId.Text;
                GlobalUtils.ChannelBLLInstance.UpdateChannel(ci);
            }
            else
            {
                GlobalUtils.ChannelBLLInstance.AddChannel(ci);
            }

            Response.Redirect("~/MgrModel/ChannelMgrList.aspx");
        }
    }
}