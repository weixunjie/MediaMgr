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
    public partial class AudioEncoderList : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public List<EncoderAudioInfo> GetAllEncoders()
        {
            List<EncoderAudioInfo> datas = GlobalUtils.EncoderBLLInstance.GetAllEncoders();

            return datas;         
                
        }

        public List<EncoderSyncGroupInfo> GetAllGroups()
        {
            List<EncoderSyncGroupInfo> reData = new List<EncoderSyncGroupInfo>();
            List<GroupInfo> datas = GlobalUtils.GroupBLLInstance.GetAllGroupsWithOutDeviceInfo();
            if (datas != null)
            {
                foreach (var d in datas)
                {
                    reData.Add(new EncoderSyncGroupInfo { groupId = d.GroupId, groupName = d.GroupName });
 
                }
            }

            return reData;

        }

    }
}