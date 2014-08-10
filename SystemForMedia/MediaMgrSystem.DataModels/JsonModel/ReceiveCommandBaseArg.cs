using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ReceiveCommandBaseArg 
    {

        public string streamName
        {
            get;
            set;
        }


        public string errorCode
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }




    }
}
