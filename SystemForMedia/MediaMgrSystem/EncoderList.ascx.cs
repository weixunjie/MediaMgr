using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using MediaMgrSystem.DataModels;
using System.Threading;
using MediaMgrSystem.BusinessLayerLogic;
namespace MediaMgrSystem
{
    public partial class EncoderList : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {


        }


        public List<EncoderInfo> GetAllEncoders()
        {
            List<EncoderInfo> datas = GlobalUtils.EncoderBLLInstance.GetAllEncoders();

            return datas;         
                
        }

    }
}