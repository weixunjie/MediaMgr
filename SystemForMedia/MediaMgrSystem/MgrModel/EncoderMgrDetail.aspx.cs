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
    public partial class EncoderMgrDetail : System.Web.UI.Page
    {

        private EncoderBLL encoderBLL = new EncoderBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;

                    EncoderInfo ei = encoderBLL.GetEncoderById(id);

                    this.TbName.Text = ei.EncoderName;


                }
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/EncoderMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            EncoderInfo ei = new EncoderInfo();


            ei.EncoderName = this.TbName.Text;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                ei.EncoderId = TbHiddenId.Text;              
                encoderBLL.UpdateEncoder(ei);
            }
            else
            {
                encoderBLL.AddEncoder(ei);
            }

            Response.Redirect("~/MgrModel/EncoderMgrList.aspx");
        }
    }
}