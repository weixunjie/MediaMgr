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
    public partial class EncoderVideoMgrDetail : System.Web.UI.Page
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



                    VideoEncoderInfo ei = GlobalUtils.VideoEncoderBLLInstance.GetEncoderById(id);

                    tbUdpAddress.Text = ei.UdpAddress ;

                    tbBoundRate.Text = ei.BaudRate;
                 
                    this.TbName.Text = ei.EncoderName;


                }
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/EncoderVideoMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            VideoEncoderInfo ei = new VideoEncoderInfo();


            ei.EncoderName = this.TbName.Text;


            ei.BaudRate = this.tbBoundRate.Text;


            ei.UdpAddress = this.tbUdpAddress.Text;

            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                ei.EncoderId = TbHiddenId.Text;
                GlobalUtils.VideoEncoderBLLInstance.UpdateEncoder(ei);
            }
            else
            {
                GlobalUtils.VideoEncoderBLLInstance.AddEncoder(ei);
            }

            Response.Redirect("~/MgrModel/EncoderVideoMgrList.aspx");
        }
    }
}