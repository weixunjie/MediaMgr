using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class DeviceOperationCommandArg
    {
        public string scheduleTurnOnTime
        {
            get;
            set;
        }

        public string scheduleShutDownTime
        {
            get;
            set;
        }

        public string volValue
        {
            get;
            set;
        }

        public int isEnabled
        {
            get;
            set;
        }


        public string newIpAddress
        {
            get;
            set;
        }

        public string serverUrl
        {
            get;
            set;
        } 


    }
}
