using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class RunningEncoder
    {
        public string ClientIdentify { get; set; }
        public string Priority { get; set; }

        public string GroupIds { get; set; }

        public string DevIds { get; set; }
    }
}
