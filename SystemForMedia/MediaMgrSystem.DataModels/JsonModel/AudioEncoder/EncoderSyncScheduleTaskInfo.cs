using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderSyncScheduleTaskInfo
    {
        public String startTime { get; set; }

        public String endTime { get; set; }

        public String weeks { get; set; }

        public String days { get; set; }

        public string fileName { get; set; }

        public string baseUrl { get; set; }

    }
}
