﻿using System;
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
        protected void Page_Load(object sender, EventArgs e)
        {


        }

        public List<ScheduleInfo> GetAllSchedules()
        {
            List<ScheduleInfo> results= GlobalUtils.ScheduleBLLInstance.GetAllSchedules();

            ScheduleInfo si = new ScheduleInfo();
            si.ScheduleId = "-1";

            si.ScheduleName = "取消计划";

            results.Add(si);

            return results;


        }

        public string GetIsPlaying()
        {
            return GlobalUtils.IsChannelManuallyPlaying.ToString().ToLower();
        }

        public string[] GetIsPlayingPIds()
        {
            return GlobalUtils.ChannelManuallyPlayingPids;
        }

        public string GetIsPlayingChannelId()
        {
            return GlobalUtils.ChannelManuallyPlayingChannelId;
        }

        public string GetIsPlayingChannelName()
        {
            return GlobalUtils.ChannelManuallyPlayingChannelName;
        }

        public string GetIsChannelManuallyPlayingIsRepeat()
        {
            return GlobalUtils.ChannelManuallyPlayingIsRepeat.ToString().ToLower();
        }


        public List<ChannelInfo> GetAllChannels()
        {
            List<ChannelInfo> channels = GlobalUtils.ChannelBLLInstance.GetAllChannels();

            return channels;

        }


        public List<ProgramInfo> GetAllPrograms()
        {
            List<ProgramInfo> pis = GlobalUtils.ProgramBLLInstance.GetAllProgram();


            return pis;

        }


    }
}