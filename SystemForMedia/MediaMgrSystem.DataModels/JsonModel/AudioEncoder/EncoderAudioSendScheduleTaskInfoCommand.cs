using MediaMgrSystem.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderAudioSendScheduleTaskInfoCommand : ComunicationBase
    {
        public List<EncoderSyncScheduleTaskInfo> tasks
        {
            get;
            set;
        }

    }


  

}
