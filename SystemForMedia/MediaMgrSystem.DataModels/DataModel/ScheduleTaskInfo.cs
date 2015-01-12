using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ScheduleTaskInfo
    {
        public String ScheduleId { get; set; }
        public String ScheduleTaskId { get; set; }

        public String ScheduleTaskName { get; set; }

        public String ScheduleTaskStartTime { get; set; }

        public String ScheduleTaskEndTime { get; set; }

        public String ScheduleTaskPriority { get; set; }

        public String ScheduleTaskProgarmId { get; set; }

        public String ScheduleTaskProgarmName { get; set; }

        public List<String> ScheduleTaskWeeks { get; set; }

        public List<String> ScheduleTaskSpecialDays { get; set; }

        public List<String> ScheduleTaskspecialDaysToWeeks { get; set; }
        public String StrWeeks { get; set; }

        public String StrDays { get; set; }

        public String IsRepeat { get; set; }

        public int IsForAudio { get; set; }
       
        public String StrSpecialDaysToWeeks { get; set; }



    }
}
