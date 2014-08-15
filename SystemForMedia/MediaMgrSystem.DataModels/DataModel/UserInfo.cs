using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ParamConfig
    {
        public int BufferTimeForManualPlay { get; set; }

        public int BufferTimeForSchedule { get; set; }

        public int IntervalTimeFromStopToPlay { get; set; }


        public int MaxClientsCountForVideo { get; set; }


        public int MaxClientsCountForAudio { get; set; }

        public int MaxClientsCountForRemoteControl{ get; set; }

    }

}
