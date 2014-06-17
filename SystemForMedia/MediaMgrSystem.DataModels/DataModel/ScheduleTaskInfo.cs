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



    }
}
