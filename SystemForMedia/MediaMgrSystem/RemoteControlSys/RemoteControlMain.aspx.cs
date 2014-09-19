
using MediaMgrSystem.BusinessLayerLogic;
using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MediaMgrSystem
{  
    public partial class RemoteControlMain : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

        }


        [WebMethod]
        public static string RangerUserControl(string controlName)
        {
            StringBuilder build = new StringBuilder();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(new StringWriter(build));
            UserControl uc = new UserControl();
            
            Control ctrl = uc.LoadControl(@"RemoteControlSys\"+controlName + ".ascx");
            htmlWriter.Flush();
            string result;
            try
            {
                ctrl.RenderControl(htmlWriter);
            }
            catch
            {
            }
            finally
            {
                htmlWriter.Flush();
                result = build.ToString();
            }
            return result;
        }


    }
}