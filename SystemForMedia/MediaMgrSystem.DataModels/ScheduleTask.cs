using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ScheduleTask
    {
        public List<string> repeatWeekDays
        {
            get;
            set;
        }

        public List<string> streamSrcs
        {
            get;
            set;
        }

        public List<string> specialDays
        {
            get;
            set;
        }

        public string beginTime
        {
            get;
            set;
        }


        public string endTime
        {
            get;
            set;
        }

        public string buffer
        {
            get;
            set;
        }

        public string isRepeat
        {
            get;
            set;
        }

    }
}
