using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class VideoServerOperArg
    {

        public List<string> streamSrcs
        {
            get;
            set;
        }

        public string streamName
        {
            get;
            set;
        }

        public string udpBroadcastAddress
        {
            get;
            set;
        }

        public string currentTime
        {
            get;
            set;
        }


        public int isRepeat
        {
            get;
            set;

        }
        public string buffer
        {
            get;
            set;
        }

    }
}
