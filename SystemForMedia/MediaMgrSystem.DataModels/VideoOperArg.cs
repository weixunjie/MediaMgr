using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class VideoOperArg 
    {

        public string streamSrc
        {
            get;
            set;
        }

        public string streamName { get; set; }
        public string udpBroadcastAddress
        {
            get;
            set;
        }

        public string destDeviceIP
        {
            get;
            set;
        }

        public int mediaType
        {
            get;
            set;
        }

        public string bitRate
        {
            get;
            set;
        }

        public string broadcastFlag
        {
            get;
            set;
        }

    }
}
