using Aladdin.HASP;
using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataAccessLayer;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem
{
    public partial class Login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {




            HaspFeature feature = HaspFeature.FromFeature(1);

            string vendorCode =
            "1ziy757gG/pwwAp+5L5WJj9E0EZRb8YEFI6AEdJ0PulID52APrEkNYYoMZAQ6qT55ZnEDjzHNoQh2n1I" +
            "EhfxyRMlGtruunxwbengMIq/DJY1WX24BpYdA2Q4AseCwBHdVWXgjjdOReoP1DbgTx53rBwS4iKsXNbl" +
            "fpuBPT3amAoT3BhHaGzGfKrLen6Z+qx/R+xLwr1joimalw5maBmryPUrpEA92SDsYQbqhwcsIPHu0E7j" +
            "NWV/NDxeSMim7J8pyENqGLQviDSg6OH3itknVff8A0FG2f1VeQPGQ5inyT8HxmdKQZQiJSYR3U4ztPHE" +
            "XtLt6OOnT5Kkfhc6DfzNkOHFjEf8gJpmhUtdknBQmuMP3ukBQl3YgjRZTkOXUVcYmXdesKXfQ7FCwyTz" +
            "meJmJbL+/99T/AJwjkH3hBQn19JR9nFjXvRZCptNaZ37NXmFu6Db/Oztnx8Fe79BID6a2KeFaP4LBPcm" +
            "SaxCWo/8Xd+c8Y9YLaVUaN25R8qPWq1KzFzUJcjTYeMlW8HHwx2krdzW8heSuGk2GOYWpqbsG4fMhOtI" +
            "s5lc6UDqoMHR4pbGxNbJORFiy7T1cynVXM1jzRlfna4OuHB7GFRZDN/Ogd/diQ+5FGKbNdNy6ZlKIAhw" +
            "xjShRRkj118T8yuUdl0bdz/pzKGH0MOj+Xf7LKLt8+b5vPyCzysdzEWd5iyP2enkNuHhzHm/Hbv7BDWs" +
            "Fio5mF+B36cgJ63YD+1oZkyRYgjKT9u9/NB8mPVm3pfvTD8DVXWs90FiCyJKL6865r0IgOcy1WEotqeB" +
            "D/klb+BhgDWo3ZUdT22KMXru+OYcf9dkhMSk74gwYZx/9F/Zfz6HuSlxlLlWdGfivWD9ygnXjTM/aCyE" +
            "pdDwd0fT9NP4W8AWqdLmCviuGmNkYfgPmphp5uemy5FyqD+/Li4nh6Uu0n1TxJF1f+m4IAVmsjWxDYgd" +
            "iPSLqRSQrQU9ky5C9tUPgQ==";

            Hasp hasp = new Hasp(feature);
            HaspStatus status = hasp.Login(vendorCode);

            if (HaspStatus.StatusOk != status)
            {
                //handle error
            }



            // Please note that there is no need to call
            // a logout function explicitly - although it is
            // recommended. The garbage collector will perform
            // the logout when disposing the object.
            // If you need more control over the logout procedure
            // perform one of the more advanced tasks.
           // return hasp.IsLoggedIn() ? hasp : null;

            //string a=EncryptUtils.DesEncrypt("30");
            //Session["IsLoginPageNow"] = true;
        }

        protected void Login_Click(object sender, EventArgs e)
        {
            this.lbMessage.Visible = false;
            UserInfo ui = GlobalUtils.UserBLLInstance.GetUserByCritiea(tbUserName.Text, tbPassword.Text);

            if (ui != null)
            {
                Session["UserId"] = ui.UserId;
                Session["UserName"] = ui.UserName;
                Session["IsSuperUser"] = ui.UserLevel == "1" ? "1" : null;
                Response.Redirect("~/AudioBroadcastMain.aspx");
            }
            else
            {
                lbMessage.Visible = true;
                //Response.Write("<script>alert('登录失败');</script>");

            }




        }


    }
}