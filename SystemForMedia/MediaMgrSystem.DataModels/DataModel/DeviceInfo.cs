using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class DeviceInfo
    {
        public String DeviceId { get; set; }

        public String DeviceName { get; set; }

        public String DeviceIpAddress { get; set; }

        public String GroupId { get; set; }

        public String GroupName { get; set; }

        public bool UsedToVideoOnline { get; set; }

        public bool UsedToAudioBroandcast { get; set; }


    }

}
