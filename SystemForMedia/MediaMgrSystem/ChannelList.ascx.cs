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
    public partial class ChannelList : System.Web.UI.UserControl
    {
        private ChannelBLL channelBLL = new ChannelBLL(GlobalUtils.DbUtilsInstance);

        private ProgramBLL programBLL = new ProgramBLL(GlobalUtils.DbUtilsInstance);

        private ScheduleBLL scheduleBLL = new ScheduleBLL(GlobalUtils.DbUtilsInstance);
        
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        public List<ScheduleInfo> GetAllSchedules()
        {
            return scheduleBLL.GetAllSchedules();

        }


        public string GetIsPlaying()
        {
            return GlobalUtils.IsChannelPlaying.ToString().ToLower();
        }


        public List<ChannelInfo> GetAllChannels()
        {
            List<ChannelInfo> channels = channelBLL.GetAllChannels();

            return channels;
                
        }


        public List<ProgramInfo> GetAllPrograms()
        {
            List<ProgramInfo> pis = programBLL.GetAllProgram();
           

            return pis;

        }

    
    }
}