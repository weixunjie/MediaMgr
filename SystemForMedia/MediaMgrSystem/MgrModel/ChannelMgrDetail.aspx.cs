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

        private ChannelBLL channelBLL = new ChannelBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    ChannelInfo ci = channelBLL.GetChannelById(id);

                    this.TbName.Text = ci.ChannelName;


                }
            }
        }

        protected void Unnamed7_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/ChannelMgrList.aspx");
        }

        protected void Unnamed6_Click(object sender, EventArgs e)
        {
            ChannelInfo ci = new ChannelInfo();


            ci.ChannelName = this.TbName.Text;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                ci.ChannelId = TbHiddenId.Text;              
                channelBLL.UpdateChannel(ci);
            }
            else
            {
                channelBLL.AddChannel(ci);
            }

            Response.Redirect("~/MgrModel/ChannelMgrList.aspx");
        }
    }
}