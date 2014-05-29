using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class GroupInfo
    {
        public String GroupId { get; set; }

        public String GroupName { get; set; }


        public List<DeviceInfo> Devices { get; set; }


    }
}
