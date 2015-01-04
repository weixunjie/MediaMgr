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
    public partial class VideoEncodeList : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public List<VideoEncoderInfo> GetAllEncoders()
        {
            List<VideoEncoderInfo> datas = GlobalUtils.VideoEncoderBLLInstance.GetAllEncoders();

            return datas;         
                
        }

        public bool CheckDeviceCallingOnline(string encoderId)
        {
        
            foreach (var st in GlobalUtils.GlobalGroupBusinessStatus)
            {
                if (st.TypeRunning == BusinessTypeForGroup.VideoEncoder)
                {
                    if (st.encoderId == encoderId)
                    {
                      //  isFound = true;
                        return true;
                    }
                }
            }

            return false;
        
        }

        public string GetImageUrl(string encoderId)
        {

            string srcName = "ic_image_video_encoder.png";

            if (CheckDeviceCallingOnline(encoderId))
            {
                srcName = "ic_image_video_encoder_billing.png";
            }



            return srcName;

        }

    }
}