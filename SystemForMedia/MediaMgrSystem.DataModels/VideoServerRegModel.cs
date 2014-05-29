using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class VideoServerRegModel : ComunicationBase
    {
        public string IpAddress
        {
            get;
            set;
        }

        public string ConnectionId
        {
            get;
            set;
        }
    }
}
