using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class SingalConnectedClient
    {
        public string ConnectionId
        {
            get;
            set;
        }
        public string ConnectionIdentify
        {
            get;
            set;
        }

        public SingalRClientConnectionType ConnectionType
        {
            get;
            set;
        }

    }
}
