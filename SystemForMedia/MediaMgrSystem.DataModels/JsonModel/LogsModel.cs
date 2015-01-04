using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class LogsModel
    {
        public string ipAddress
        {
            get;
            set;
        }

        public List<string> values
        {
            get;
            set;
        }    

    }
}
