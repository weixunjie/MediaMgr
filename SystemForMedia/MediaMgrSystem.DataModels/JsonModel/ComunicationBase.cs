using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaMgrSystem.DataModels
{
    public class ComunicationBase
    {
        public string guidId
        {
            get;
            set;
        }

        public CommandTypeEnum commandType
        {
            get;
            set;
        }    

    }
}
