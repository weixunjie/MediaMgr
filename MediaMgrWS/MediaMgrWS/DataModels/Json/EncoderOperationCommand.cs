using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrWS
{


    public class EncoderOperationCommand : EncoderCommandBase
    {
        public List<string> groupIds
        {
            get;
            set;
        }


    }

}
