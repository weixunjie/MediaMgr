using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ScheduleRequest
    {
        public string guidId
        {
            get;
            set;
        }

        public string commandType
        {
            get;
            set;
        }


        public List<ScheduleTask> streamSrcs
        {

            get;
            set;
        }

    }
}
