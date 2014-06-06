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
namespace MediaMgrSystem
{
    public partial class ChannelList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<ScheduleInfo> GetAllSchedules()
        {
            List<ScheduleInfo> schedules = new List<ScheduleInfo>();

            ScheduleInfo gi = new ScheduleInfo { ScheduleId = "1", ScheduleName = "moren" };

            schedules.Add(gi);
            schedules.Add(new ScheduleInfo { ScheduleId = "2", ScheduleName = "ddf" });

            return schedules;

        }

        


        public List<ChannelInfo> GetAllChanels()
        {
            List<ChannelInfo> channels = new List<ChannelInfo>();

            ChannelInfo gi = new ChannelInfo { ChannelId = "1", ChannelName = "moren" };



            channels.Add(gi);
            channels.Add(new ChannelInfo { ChannelId = "1", ChannelName = "ddf" });

            channels.Add(new ChannelInfo { ChannelId = "23", ChannelName = "ddjklf" });

                     channels.Add(new ChannelInfo { ChannelId = "283", ChannelName = "hkjhk" });
            return channels;

        }





    }
}