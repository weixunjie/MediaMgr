﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class VideoOperAndriodClientArg
    {
        public List<string> streamSrcs
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

    }
}
