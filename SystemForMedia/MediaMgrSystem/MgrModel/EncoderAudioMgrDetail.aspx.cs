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
    public partial class EncoderAudioMgrDetail : System.Web.UI.Page
    {

        private EncoderAudioBLL encoderBLL = new EncoderAudioBLL(GlobalUtils.DbUtilsInstance);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!Page.IsPostBack)
            {

                ddPriority.Items.Clear();

                for (int i = 1; i <= 10; i++)
                {
                    ddPriority.Items.Add(new ListItem { Text = i.ToString(), Value = i.ToString() });
                }

                if (Request["id"] != null)
                {
                    string id = Request["id"].ToString();

                    TbHiddenId.Text = id;



                    EncoderAudioInfo ei = encoderBLL.GetEncoderById(id);

                    tbIpAddress.Text = ei.ClientIdentify;

                    tbBoundRate.Text = ei.BaudRate;

                    ddPriority.SelectedValue = ei.Priority;
                    this.TbName.Text = ei.EncoderName;


                }
            }
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/MgrModel/EncoderAudioMgrList.aspx");
        }

        protected void Add_Click(object sender, EventArgs e)
        {
            EncoderAudioInfo ei = new EncoderAudioInfo();


            ei.EncoderName = this.TbName.Text;


            ei.BaudRate = this.tbBoundRate.Text;



            if (!string.IsNullOrEmpty(TbHiddenId.Text))
            {
                ei.EncoderId = TbHiddenId.Text;
                encoderBLL.UpdateEncoder(ei);
            }
            else
            {
                encoderBLL.AddEncoder(ei);
            }

            Response.Redirect("~/MgrModel/EncoderAudioMgrList.aspx");
        }
    }
}