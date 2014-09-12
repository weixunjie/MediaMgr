using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class EncoderAudioCommandBase
    {
        public string guidId
        {
            get;
            set;
        }

        public CommandTypeEnum commandType
        {
            get;
            set;
        }
        public string clientIdentify
        {
            get;
            set;
        }

    }
}
