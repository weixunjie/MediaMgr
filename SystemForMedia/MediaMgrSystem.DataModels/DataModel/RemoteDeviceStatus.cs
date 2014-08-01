using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class RemoteDeviceStatus
    {
        public string ClientIdentify
        {
            get;
            set;
        }
        public RemoveControlDeviceType DeviceType
        {
            get;
            set;
        }

        public bool DeviceOpenedStatus
        {
            get;
            set;
        }

        public string ACTempature
        {
            get;
            set;
        }

        public string ACMode
        {
            get;
            set;
        }

        
        

    }
}
