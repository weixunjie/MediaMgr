using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class SyncTimeCommandArg
    {
        public string serverNowTime
        {
            get;
            set;
        }

        public string upgradeVer
        {
            get;
            set;
        }

        public string upgradeUrl
        {
            get;
            set;
        }
    }
}
