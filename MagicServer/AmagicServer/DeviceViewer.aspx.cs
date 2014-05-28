using AmagicServer.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AmagicServer
{
    public partial class DeviceViewer : System.Web.UI.Page
    {
        private DeviceInfoBLL deviceInfoBLL = new DeviceInfoBLL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null || string.IsNullOrEmpty(Session["UserId"].ToString()))
            {
                Response.Redirect("Account/Login.aspx");
            }

            if (!Page.IsPostBack)
            {

                if (string.IsNullOrEmpty(tbCriteia.Text))
                {
                    dvDevice.DataSource = deviceInfoBLL.GetAllDevices();
                    dvDevice.DataBind();
                }
                else
                {
                    dvDevice.DataSource = deviceInfoBLL.GetAllDeviceByPhoneNoCloseMatch(tbCriteia.Text);
                    dvDevice.DataBind();
                }

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(tbCriteia.Text))
            {
                dvDevice.DataSource = deviceInfoBLL.GetAllDevices();
                 dvDevice.DataBind();
            }
            else
            {
                dvDevice.DataSource = deviceInfoBLL.GetAllDeviceByPhoneNoCloseMatch(tbCriteia.Text);
                dvDevice.DataBind();
            }
        }
    }
}