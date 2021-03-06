﻿
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
    public partial class RemoteControlDetailList : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }


        }


        public List<RemoteDeviceStatus> GetAllStatus()
        {
            GlobalUtils.AllRemoteDeviceStatus = GlobalUtils.RemoteDeviceStatusBLLInstance.GetAllStatus();

            return GlobalUtils.AllRemoteDeviceStatus;

        }

        public string GetStatusTextByIdentifyAndDeviceType(string identify, RemoveControlDeviceType type)
        {
            if (GlobalUtils.AllRemoteDeviceStatus != null && GlobalUtils.AllRemoteDeviceStatus.Count > 0)
            {
                foreach (var status in GlobalUtils.AllRemoteDeviceStatus)
                {
                    if (status.DeviceType == type && status.ClientIdentify == identify)
                    {
                      
                        if (status.DeviceOpenedStatus)
                        {                           


                            return "已打开";

                        }
                        else
                        {
                         

                            return "已关闭";
                        }

                       
                    }
                }
            }

           
            return "未知";

        }

        public List<GroupInfo> GetAllGroups()
        {
            List<GroupInfo> gis = GlobalUtils.GroupBLLInstance.GetAllGroupsByBusinessType(BusinessType.REMOVECONTROL);

            return gis;

        }
    }
}