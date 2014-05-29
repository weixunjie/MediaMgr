using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class VideoServerOperCommand : ComunicationBase
    {
        
        public VideoServerOperArg arg
        {
            get;
            set;
        }

    }
}
