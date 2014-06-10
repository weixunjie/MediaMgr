using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication8
{
    public partial class expresscheck : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                NVPAPICaller test = new NVPAPICaller();
                string retMsg = "";
                string token = "";
                //string payerId = "";


                bool ret = test.ShortcutExpressCheckout("0.1",Request["DNO"].ToString(), ref token, ref retMsg);
                if (ret)
                {
                    //Session["token"] = token;
                    Response.Redirect(retMsg);
                }
                else
                {
                    Response.Redirect("APIError.aspx?" + retMsg);
                }
            }

        }
    }
}