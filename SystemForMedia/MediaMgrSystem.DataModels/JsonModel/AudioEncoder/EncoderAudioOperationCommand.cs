using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{


    public class EncoderAudioOperationCommand : EncoderAudioCommandBase
    {
        public List<string> groupIds
        {
            get;
            set;
        }


    }

}
