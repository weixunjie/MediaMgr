using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderOpenCommandeArg
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
        // ":"udp://229.0.0.1:5000", “”:”123789101”,   ”:”100”} 
    }
    public class EncoderOpenReponse : ComunicationBase
    {

        public EncoderOpenCommandeArg arg
        {
            get;
            set;
        }
    }
}
