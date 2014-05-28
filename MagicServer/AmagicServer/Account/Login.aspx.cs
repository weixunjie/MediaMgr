using AmagicServer.BLL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AmagicServer.Account
{
    public partial class Login : Page
    {
        private DbUitls dbUitls = new DbUitls();

        protected void Page_Load(object sender, EventArgs e)
        {
           
            //RegisterHyperLink.NavigateUrl = "Register.aspx";
            //OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];

            //var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            //if (!String.IsNullOrEmpty(returnUrl))
            //{
            //    RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            //}
        }

        protected void Unnamed6_Click(object sender, EventArgs e)
        {


         
            string selectStr = "select * from UserInfo where UserName='{0}' and Password='{1}'";


            selectStr = String.Format(selectStr, UserName.Text, Password.Text);

            using (SqlDataReader reader = dbUitls.GetSqlRedaer(selectStr))
            {
                if (reader != null && reader.Read())
                {
                    Session["UserId"] = UserName.Text;

            

                    FormsAuthentication.SetAuthCookie(  Session["UserId"].ToString() , false);
                    dbUitls.ClosoConnection();
                    Response.Redirect(@"..\DeviceViewer.aspx");
                    
                }
            }



          //  string sqlStr = 
            
        }
    }
}