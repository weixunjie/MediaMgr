using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class DeviceRemoteControlManualCommand : ComunicationBase
    {
        public DeviceRemoteControlManualArg arg
        {
            get;
            set;
        }      

    }
}
