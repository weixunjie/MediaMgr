using AmagicServer.BLL;
using AmagicServer.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace AmagicServer
{
    public partial class CancelResult : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write(System.Environment.NewLine + "<br/> 您已取消付款。 <br/>"); 
        }  
    }
}