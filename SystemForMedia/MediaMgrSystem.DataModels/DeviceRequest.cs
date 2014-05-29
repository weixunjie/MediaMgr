using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class DeviceRequest
    {
        public string guidId
        {
            get;
            set;
        }

        public string commandType
        {
            get;
            set;
        }

        public string deviceIP
        {
            get;
            set;
        }

        public string broadcastFlag
        {
            get;
            set;
        }


        public VideoOperCommand arg
        {
            get;
            set;
        }

        public List<String> streamSrcs
        {

            get;
            set;
        }

    }
}
