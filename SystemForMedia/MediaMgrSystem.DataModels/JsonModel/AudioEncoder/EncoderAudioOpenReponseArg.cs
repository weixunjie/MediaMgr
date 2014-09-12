using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderOpenReponseArg
    {
        public string udpBroadcastAddress
        {
            get;
            set;
        }

        public string streamName
        {
            get;
            set;

        }
        public string baudRate
        {
            get;
            set;

        }
      
    }

}
