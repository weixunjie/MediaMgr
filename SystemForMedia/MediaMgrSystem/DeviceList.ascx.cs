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
    public partial class DeviceList : System.Web.UI.UserControl
    {

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public List<GroupInfo> GetAllGroupsIncludeDefaultGroup()
        {
            List<GroupInfo> gis = null;



            gis = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(GlobalUtils.GetCurrentFunctionType());



            GroupInfo groupDefault = new GroupInfo();

            groupDefault.GroupId = "-1";
            groupDefault.GroupName = "默认分组";




            groupDefault.Devices = GlobalUtils.DeviceBLLInstance.GetAllDevicesByGroupWithFilter("-1", GlobalUtils.GetCurrentFunctionType());





            gis.Insert(0, groupDefault);

            return gis;

        }


        public bool CheckIfPlaying()
        {
            return GlobalUtils.CheckIfPlaying();

        }

        public bool CheckIsSupperUser()
        {
            if (HttpContext.Current != null &&
                HttpContext.Current.Session["IsSuperUser"] != null
                && HttpContext.Current.Session["IsSuperUser"].ToString() == "1")
            {
                return true;
            }

            return false;


        }





        public string GetGroupImageUrl()
        {
            string srcName = "ic_image_group.png";
            if (GlobalUtils.GetCurrentFunctionType() == BusinessType.VIDEOONLINE) { srcName = "ic_image_group_video.png"; }


            return srcName;
        }
        public string GetImageUrl(string ipAddress)
        {

            string srcName = "ic_image_device";

            if (!CheckDeviceIsOnline(ipAddress))
            {
                srcName = "ic_image_device_offline";
            }





            if (GlobalUtils.GetCurrentFunctionType() == BusinessType.VIDEOONLINE)
            {
                srcName = srcName + "_video";
            }

            //bool isAudio = false;

            //if (CheckDeviceIsPlaying(ipAddress, out isAudio))
            //{
            //    if (isAudio)
            //    {
            //        srcName = "ic_image_device.playing_audio";

            //    }
            //    else
            //    {
            //        srcName = "ic_image_device.playing_video";
            //    }

            //}


            return srcName;

        }
        public bool CheckDeviceIsOnline(string ipAddress)
        {
            List<string> ipReallySent = new List<string>();
            return GlobalUtils.GetConnectionIdsByIdentify(new List<string> { ipAddress }, SingalRClientConnectionType.ANDROID, out ipReallySent).Count > 0;

        }

        public bool CheckDeviceIsPlaying(string ipAddress, out bool isAudio)
        {
            isAudio = false;
            foreach (var de in GlobalUtils.PlayingDevices)
            {
                if (ipAddress == de.IpAddress)
                {
                    isAudio = de.IsAudio;
                    return true;
                }
            }

            return false;


        }

        public void GetAllEncoderRunning()
        {
            GlobalUtils.RunningEncoder = GlobalUtils.EncoderAudioRunningClientsBLLInstance.GetAllEncoderRunning();
        }

        public string CheckDeviceIsGroupPlaying(string groupId)
        {

            string strPlaying = "(播放中)";
            foreach (var mp in GlobalUtils.ManualPlayItems)
            {
                if (mp.ChannelGroup != null)
                {
                    foreach (var gi in mp.ChannelGroup)
                    {
                        if (gi.GroupId == groupId)
                        {
                           // outPlayingType = "audio";
                            return strPlaying;
                        }
                    }
                }
                    
            }

            foreach (var rs in GlobalUtils.RunningSchudules)
            {
                if (rs.ChannelGroup != null)
                {
                    foreach (var gi in rs.ChannelGroup)
                    {
                        if (gi.GroupId == groupId)
                        {
                           // outPlayingType = "audio";
                            return strPlaying;
                        }
                    }
                }

            }


            foreach (var rs in GlobalUtils.RunningVideoEncoder)
            {
                if (rs.Groups != null)
                {
                    foreach (var gi in rs.Groups)
                    {
                        if (gi.GroupId == groupId)
                        {
                            // outPlayingType = "audio";
                            return strPlaying;
                        }
                    }
                }

            }

            List<RunningEncoder> runs = GlobalUtils.RunningEncoder; 

            foreach (var rs in runs)
            {
                
                if (rs.GroupIds != null)
                {
                    String[] strs=rs.GroupIds.Split(',');
                    foreach (var gi in strs)
                    {
                        if (gi == groupId)
                        {
                            // outPlayingType = "audio";
                            return strPlaying;
                        }
                    }
                }

            }

            return "";


        }



        public List<ChannelInfo> GetAllChannels()
        {
            ChannelInfo ci = new ChannelInfo();

            ci.ChannelName = "-1";
            ci.ChannelId = "无";

            List<ChannelInfo> res = GlobalUtils.ChannelBLLInstance.GetAllChannels();

            res.Insert(0, ci);

            return res;
        }

        public List<VideoEncoderInfo> GetAllEncoders()
        {

            VideoEncoderInfo ve = new VideoEncoderInfo();

            ve.EncoderId = "-1";
            ve.EncoderName = "无";

            List<VideoEncoderInfo> res = GlobalUtils.VideoEncoderBLLInstance.GetAllEncoders();
            res.Insert(0, ve);
            return res;

        }


    }
}