using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MediaMgrSystem
{
    /// <summary>
    /// Summary description for AudioLogic
    /// </summary>
    public class AudioLogic : IHttpHandler,System.Web.SessionState.IRequiresSessionState
    {
               
        public void ProcessRequest(HttpContext context)
        {        

            if (context.Request["sessInterval"] != null && context.Request["sessInterval"].ToString() == "Y")
            {
                if (context.Session["UserId"] == null)
                {
                    context.Response.Redirect(context.Server.MapPath("~/Login.aspx"));
                    return;
                }

                return;
            }
            
            context.Response.ContentType = "text/plain";


            StringBuilder build = new StringBuilder();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(new StringWriter(build));
            UserControl uc = new UserControl();
            Control ctrl = uc.LoadControl("DeviceList.ascx");
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

            context.Response.Write(result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}