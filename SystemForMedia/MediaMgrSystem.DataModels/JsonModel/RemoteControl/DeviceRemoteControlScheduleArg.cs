using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class DeviceRemoteControlScheduleArg
    {
        public string devicesType
        {
            get;
            set;
        }

          public string scheduleTime
        {
            get;
            set;
        }

          public string weekDays
        {
            get;
            set;
        }

        


        public DeviceRemoteControlACParam paramsData
        {
            get;
            set;
        }

    }
}
