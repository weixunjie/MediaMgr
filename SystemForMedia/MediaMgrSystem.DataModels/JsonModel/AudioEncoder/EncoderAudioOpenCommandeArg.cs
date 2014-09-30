using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderVideoOperationCommandeArg
    {
        public string udpBroadcastAddress
        {
            get;
            set;
        }
            
        public string biteRate
        {
            get;
            set;

        }

        public string mediaType
        {
            get;
            set;
        }

    }

   
}
